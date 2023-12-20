export function loadImage(data) {
    const buffer = new Uint8Array(data);
    const blob = new Blob([buffer]);
    const url = URL.createObjectURL(blob);
    const image = document.getElementById("texture");
    image.onload = (e) => {
        URL.revokeObjectURL(url);
    };
    image.src = url;
}
//# sourceMappingURL=Index.razor.js.map