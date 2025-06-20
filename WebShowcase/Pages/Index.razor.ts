export async function initWebGPU() {
	if (!navigator.gpu) {
		alert("WebGPU APIs are not supported.");
		return;
	}

	const adapter: GPUAdapter | null = await navigator.gpu.requestAdapter();
	if (!adapter) {
		alert("Couldn't request WebGPU adapter.");
		return;
	}

	const device = await adapter.requestDevice();

	const canvas: HTMLCanvasElement = document.getElementById("webgpu-canvas") as HTMLCanvasElement;
	const context: GPUCanvasContext = canvas.getContext("webgpu");
	if (!context) {
		alert("Couldn't create a WebGPU context.");
		return;
	}
	console.log('WEBGPU dioleso');
}