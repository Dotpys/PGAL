import { Mat4x4, mat4x4identity, mat4x4inv, mat4x4mul, mat4x4orthogonalProjection, mat4x4rotation, quatRotation } from "/dist/matrix.js";

const shaderCode = `
// Argomenti "globali" passati alla shader
@group(0) @binding(0) var<uniform> t: f32;

@group(1) @binding(0) var<uniform> model_matrix: mat4x4<f32>;
@group(1) @binding(1) var<uniform> view_matrix: mat4x4<f32>;
@group(1) @binding(2) var<uniform> projection_matrix: mat4x4<f32>;

@group(2) @binding(0) var<uniform> light_position: vec3<f32>;

struct VertexOut {
	@builtin(position) position: vec4<f32>,
	@location(0) color: vec4<f32>,
	@location(1) normal: vec4<f32>,
	@location(2) vertex_pos: vec4<f32>
}

@vertex
fn vertex_main(
	@location(0) position: vec3f,
	@location(1) normal: vec3f
) -> VertexOut {
	_ = t;
	_ = model_matrix;
	_ = view_matrix;
	_ = projection_matrix;

	var output: VertexOut;
	output.position = ((vec4f(position, 1.0f) * model_matrix) * view_matrix) * projection_matrix;
	output.color = vec4f((position/2.0f) + vec3<f32>(0.5f, 0.5f, 0.5f) , 1.0f);
	output.normal = vec4f(normal , 1.0f) * model_matrix;
	output.vertex_pos = vec4f(position, 1.0f) * model_matrix;
	return output;
}

@fragment
fn fragment_main(fragment_data: VertexOut) -> @location(0) vec4<f32> {
	_ = light_position;

	// Basic lighting implementation
	// Ambient
	const ambient_strength = 0.1;
	const ambient = ambient_strength;

	// Diffuse
	const diffuse_strength = 0.9;

	let fragment_position = fragment_data.vertex_pos;
	let fragment_normal = normalize(fragment_data.normal);
	let light_position = vec4<f32>(-2, 3, 3, 1);

	let light_direction = normalize(light_position - fragment_position);
	let light_magnitude = dot(fragment_normal, light_direction);

	let diffuse = diffuse_strength * max(light_magnitude, 0);

	// Final
	return vec4<f32>(1.0, 1.0, 1.0, 1.0) * (ambient + diffuse);
}
`;

let device: GPUDevice | null = null;
let vertexBuffer: GPUBuffer | null = null;

let mousedown: boolean = false;
let angle: number = 0.0;

export async function startRenderer() {
	if (!navigator.gpu) {
		alert("WebGPU APIs are not supported.");
		return;
	}

	// Requests a GPU adapter.
	const adapter: GPUAdapter | null = await navigator.gpu.requestAdapter();
	if (!adapter) {
		alert("Couldn't request WebGPU adapter.");
		return;
	}
	// Requests a GPU device.
	device = await adapter.requestDevice();
	const canvas: HTMLCanvasElement = document.getElementById("viewport") as HTMLCanvasElement;
	const context: GPUCanvasContext | null = canvas.getContext("webgpu");

	if (!context) {
		alert("Couldn't create a GPU context.");
		return;
	}

	const vertices = new Float32Array([
		// -X
		-1, -1, -1, -1,  0,  0,
		-1,  1, -1, -1,  0,  0,
		-1, -1,  1, -1,  0,  0,
		-1, -1,  1, -1,  0,  0,
		-1,  1, -1, -1,  0,  0,
		-1,  1,  1, -1,  0,  0,
		// +X
		 1, -1,  1,  1,  0,  0,
		 1,  1,  1,  1,  0,  0,
		 1, -1, -1,  1,  0,  0,
		 1, -1, -1,  1,  0,  0,
		 1,  1,  1,  1,  0,  0,
		 1,  1, -1,  1,  0,  0,
		 // -Y
		-1, -1, -1,  0, -1,  0,
		-1, -1,  1,  0, -1,  0,
		 1, -1, -1,  0, -1,  0,
		 1, -1, -1,  0, -1,  0,
		-1, -1,  1,  0, -1,  0,
		 1, -1,  1,  0, -1,  0,
		 // +Y
		-1,  1,  1,  0,  1,  0,
		-1,  1, -1,  0,  1,  0,
		 1,  1,  1,  0,  1,  0,
		 1,  1,  1,  0,  1,  0,
		-1,  1, -1,  0,  1,  0,
		 1,  1, -1,  0,  1,  0,
		 // -Z
		 1, -1, -1,  0,  0, -1,
		 1,  1, -1,  0,  0, -1,
		-1, -1, -1,  0,  0, -1,
		-1, -1, -1,  0,  0, -1,
		 1,  1, -1,  0,  0, -1,
		-1,  1, -1,  0,  0, -1,
		// +Z
		-1, -1,  1,  0,  0,  1,
		-1,  1,  1,  0,  0,  1,
		 1, -1,  1,  0,  0,  1,
		 1, -1,  1,  0,  0,  1,
		-1,  1,  1,  0,  0,  1,
		 1,  1,  1,  0,  0,  1
	]);

	vertexBuffer = device.createBuffer(
		{
			label: `vertex_buffer_mesh`,
			size: vertices.byteLength,
			usage: GPUBufferUsage.VERTEX | GPUBufferUsage.COPY_DST
		}
	);
	device.queue.writeBuffer(vertexBuffer, 0, vertices);

	const timeBuffer = device.createBuffer(
		{
			label: `uniform_buffer_time`,
			size: 4,
			usage: GPUBufferUsage.UNIFORM | GPUBufferUsage.COPY_DST
		}
	);

	const modelMatrixBuffer = device.createBuffer(
		{
			label: `uniform_buffer_matrix_model`,
			size: 16 * 4,
			usage: GPUBufferUsage.UNIFORM | GPUBufferUsage.COPY_DST
		}
	);

	const viewMatrixBuffer = device.createBuffer(
		{
			label: `uniform_buffer_matrix_view`,
			size: 16 * 4,
			usage: GPUBufferUsage.UNIFORM | GPUBufferUsage.COPY_DST
		}
	);

	const projectionMatrixBuffer = device.createBuffer(
		{
			label: `uniform_buffer_matrix_projection`,
			size: 16 * 4,
			usage: GPUBufferUsage.UNIFORM | GPUBufferUsage.COPY_DST
		}
	);

	const lightBuffer = device.createBuffer(
		{
			label: `uniform_buffer_light`,
			size: 3 * 4,
			usage: GPUBufferUsage.UNIFORM | GPUBufferUsage.COPY_DST
		}
	);

	const shaderModule: GPUShaderModule = device.createShaderModule(
		{
			code: shaderCode,
			// TODO: check hint & stuff
		}
	);

	context.configure(
		{
			device: device,
			format: navigator.gpu.getPreferredCanvasFormat(),
			alphaMode: "premultiplied"
		}
	);

	const timeBindGroupLayout = device.createBindGroupLayout(
		{
			label: "bind_group_layout_time",
			entries: [
				{ binding: 0, buffer: { type: "uniform" }, visibility: GPUShaderStage.VERTEX | GPUShaderStage.FRAGMENT }
			]
		}
	);

	const timeBindGroup = device.createBindGroup(
		{
			label: "bind_group_time",
			layout: timeBindGroupLayout,
			entries: [
				{ binding: 0, resource: { buffer: timeBuffer } }
			]
		}
	);

	const matrixBindGroupLayout = device.createBindGroupLayout(
		{
			label: "bind_group_layout_matrix",
			entries: [
				{ binding: 0, buffer: { type: "uniform" }, visibility: GPUShaderStage.VERTEX },
				{ binding: 1, buffer: { type: "uniform" }, visibility: GPUShaderStage.VERTEX },
				{ binding: 2, buffer: { type: "uniform" }, visibility: GPUShaderStage.VERTEX }
			]
		}
	);

	const matrixBindGroup = device.createBindGroup(
		{
			label: "bind_group_matrix",
			layout: matrixBindGroupLayout,
			entries: [
				{ binding: 0, resource: { buffer: modelMatrixBuffer } },
				{ binding: 1, resource: { buffer: viewMatrixBuffer } },
				{ binding: 2, resource: { buffer: projectionMatrixBuffer } }
			]
		}
	);

	const lightBindGroupLayout = device.createBindGroupLayout(
		{
			label: "bind_group_layout_light",
			entries: [
				{ binding: 0, buffer: { type: "uniform" }, visibility: GPUShaderStage.FRAGMENT }
			]
		}
	);

	const lightBindGroup = device.createBindGroup(
		{
			label: "bind_group_light",
			layout: lightBindGroupLayout,
			entries: [
				{ binding: 0, resource: { buffer: lightBuffer } }
			]
		}
	);

	const byteBindGroupLayout = device.createBindGroupLayout(
		{
			label: "bind_group_layout_byte",
			entries: [
				{ binding: 0, buffer: { type: "uniform" }, visibility: GPUShaderStage.VERTEX | GPUShaderStage.FRAGMENT }
			]
		}
	);

	const pipelineLayout = device.createPipelineLayout(
		{
			label: "pipeline_layout_marching",
			bindGroupLayouts: [
				timeBindGroupLayout,
				matrixBindGroupLayout,
				lightBindGroupLayout
			]
		}
	);

	const pipelineDescriptor: GPURenderPipelineDescriptor = {
		label: "render_pipeline_marching",
		vertex: {
			module: shaderModule,
			entryPoint: "vertex_main",
			buffers: [
				{
					arrayStride: 24,
					attributes: [
						{ shaderLocation: 0, format: "float32x3", offset: 0  },
						{ shaderLocation: 1, format: "float32x3", offset: 12 },
					]
				}
			]
		},
		fragment: {
			module: shaderModule,
			entryPoint: "fragment_main",
			targets: [
				{
					format: navigator.gpu.getPreferredCanvasFormat()
				}
			]
		},
		primitive: {
			topology: "triangle-list",
			cullMode: "front",
			frontFace: "cw"
		},
		depthStencil: {
			depthWriteEnabled: true,
			depthCompare: "less",
			format: "depth24plus"
		},
		layout: pipelineLayout
	};

	const depthTexture = device.createTexture(
		{
			size: [canvas.width, canvas.height],
			format: "depth24plus",
			usage: GPUTextureUsage.RENDER_ATTACHMENT
		}
	);

	const renderPipeline = device.createRenderPipeline(pipelineDescriptor);

	const renderPassDescriptor: GPURenderPassDescriptor = {
		colorAttachments: [
			{
				view: undefined, //Assigned later
				clearValue: { r: 0.0, g: 0.0, b: 0.0, a: 0.0 },
				loadOp: "clear",
				storeOp: "store",
			}
		],
		depthStencilAttachment: {
			view: depthTexture.createView(),
			depthClearValue: 1.0,
			depthLoadOp: "clear",
			depthStoreOp: "store"
		}
	};

	// Initialize matrices
	function setModelRotation(t: number) {
		device.queue.writeBuffer(modelMatrixBuffer, 0, new Float32Array(
			mat4x4mul(
				mat4x4rotation(quatRotation(35.27, [1, 0, 0])),
				mat4x4rotation(quatRotation(angle*10 + 45, [0, 1, 0]))
			)
		).buffer, 0, 4 * 16);
	}
	// View matrix is calculated per frame.
	function setCameraPosition(x: number, y: number, z: number, t: number) {
		const translationMatrix: Mat4x4 = [
			1, 0, 0, 0,
			0, 1, 0, 0,
			0, 0, 1, 3,
			0, 0, 0, 1
		];
		device.queue.writeBuffer(viewMatrixBuffer, 0, new Float32Array(mat4x4inv(translationMatrix)).buffer, 0, 4 * 16);
	}
	device.queue.writeBuffer(projectionMatrixBuffer, 0, new Float32Array(
		mat4x4orthogonalProjection(-2, 2, -2, 2, 0.01, 20.01)
	).buffer, 0, 4 * 16);
	// Set light position
	device.queue.writeBuffer(lightBuffer, 0, new Float32Array([-2, 3, -3]).buffer, 0, 4 * 3);

	canvas.onmousedown = (event: MouseEvent) => {
		mousedown = true;
	};

	canvas.onmouseup = (event: MouseEvent) => {
		mousedown = false;
	};

	canvas.onmousemove = (event: MouseEvent) => {
		if (mousedown) {
			angle += event.movementX * 0.1;
		}
	};

	function frame(timestamp: number) {
		const t = timestamp / 1000;
		// Update vram data (uniforms)
		device.queue.writeBuffer(timeBuffer, 0, new Float32Array([t]).buffer, 0, 4);
		setModelRotation(t);
		setCameraPosition(3 * -Math.cos(t), 3, 3 * Math.sin(t), t);

		const commandEncoder = device.createCommandEncoder();

		renderPassDescriptor.colorAttachments[0].view = context!.getCurrentTexture().createView();
		const passEncoder = commandEncoder.beginRenderPass(renderPassDescriptor);
		// This draws the marching cubes
		passEncoder.setPipeline(renderPipeline);
		passEncoder.setBindGroup(0, timeBindGroup);
		passEncoder.setBindGroup(1, matrixBindGroup);
		passEncoder.setBindGroup(2, lightBindGroup);
		passEncoder.setVertexBuffer(0, vertexBuffer);
		passEncoder.draw(vertexBuffer.size / 24);
		passEncoder.end();

		device.queue.submit([commandEncoder.finish()]);

		requestAnimationFrame(frame);
	}

	requestAnimationFrame(frame);
}

export function loadVertexBuffer(data: number[]) {
	let byteArray = new Uint8Array(data);
	var floats = new Float32Array(byteArray.buffer);
	vertexBuffer = device.createBuffer(
		{
			label: `vertex_buffer_mesh`,
			size: byteArray.byteLength,
			usage: GPUBufferUsage.VERTEX | GPUBufferUsage.COPY_DST
		}
	);
	device.queue.writeBuffer(vertexBuffer, 0, byteArray);
}

export function loadFrame(data: number[]): string {
	const buffer: Uint8Array = new Uint8Array(data);
	const blob = new Blob([buffer]);
	const url = URL.createObjectURL(blob);
	const image: HTMLImageElement = <HTMLImageElement>document.getElementById("texture");
	image.onload = (e) => {
		URL.revokeObjectURL(url);
	}
	return url;
}
