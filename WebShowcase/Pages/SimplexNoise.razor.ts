export function loadImage(data: number[]) {
	const buffer: Uint8Array = new Uint8Array(data);
	const blob = new Blob([buffer]);
	const url = URL.createObjectURL(blob);
	const image: HTMLImageElement = <HTMLImageElement>document.getElementById("texture");
	image.onload = (e) => {
		URL.revokeObjectURL(url);
	}
	image.src = url;
}
