export function loadFrame(data) {
	const buffer = new Uint8Array(data);
	const blob = new Blob([buffer]);
	const url = URL.createObjectURL(blob);
	const image = document.getElementById("texture");
	//image.onload = (e) => {
	//	URL.revokeObjectURL(url);
	//}
	return url;
}
