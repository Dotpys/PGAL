export function getSVGClickPosition(clientX, clientY) {
    var svgElement = document.getElementById("viewport");
    var rect = svgElement.getBoundingClientRect();
    return [(clientX - rect.left) / (rect.right - rect.left) * 100, (clientY - rect.top) / (rect.bottom - rect.top) * 100];
}
