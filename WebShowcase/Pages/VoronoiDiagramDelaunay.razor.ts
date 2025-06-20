export function getSVGClickPosition(clientX: number, clientY: number): number[] {
	var svgElement = document.getElementById("viewport") as HTMLOrSVGImageElement;
	var rect = svgElement.getBoundingClientRect();
	return [(clientX - rect.left) / (rect.right - rect.left) * 100, (clientY - rect.top) / (rect.bottom - rect.top) * 100];
}
