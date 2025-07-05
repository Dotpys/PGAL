export function loadImage(canvasId, data, textureSize) {
    const canvas = document.getElementById(canvasId);
    canvas.width = textureSize;
    canvas.height = textureSize;
    const context = canvas.getContext("2d");
    const imageData = new ImageData(new Uint8ClampedArray(data), textureSize, textureSize);
    context.putImageData(imageData, 0, 0);
}
