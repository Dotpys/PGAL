export function loadImage(canvasId: string, data: number[], textureSize: number) {
	const canvas = document.getElementById(canvasId) as HTMLCanvasElement;
	canvas.width = textureSize;
	canvas.height = textureSize;
	const context = canvas.getContext("2d");
	const imageData = new ImageData(new Uint8ClampedArray(data), textureSize, textureSize);
	context.putImageData(imageData, 0, 0);
}
