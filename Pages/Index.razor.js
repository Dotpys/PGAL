var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
export function initWebGPU() {
    return __awaiter(this, void 0, void 0, function* () {
        if (!navigator.gpu) {
            alert("WebGPU APIs are not supported.");
            return;
        }
        const adapter = yield navigator.gpu.requestAdapter();
        if (!adapter) {
            alert("Couldn't request WebGPU adapter.");
            return;
        }
        const device = yield adapter.requestDevice();
        const canvas = document.getElementById("webgpu-canvas");
        const context = canvas.getContext("webgpu");
        if (!context) {
            alert("Couldn't create a WebGPU context.");
            return;
        }
        console.log('WEBGPU dioleso');
    });
}
