export function mat4x4identity() {
    return [
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1
    ];
}
export function mat4x4translation(transform) {
    return [
        1, 0, 0, transform[0],
        0, 1, 0, transform[1],
        0, 0, 1, transform[2],
        0, 0, 0, 1
    ];
}
export function quatRotation(angle, axis) {
    const a = (Math.PI / 180) * angle / 2;
    return [
        Math.cos(a),
        Math.sin(a) * axis[0],
        Math.sin(a) * axis[1],
        Math.sin(a) * axis[2]
    ];
}
export function mat4x4rotation(rotation) {
    const w = rotation[0];
    const x = rotation[1];
    const y = rotation[2];
    const z = rotation[3];
    const xx = x * x;
    const xy = x * y;
    const xz = x * z;
    const xw = x * w;
    const yy = y * y;
    const yz = y * z;
    const yw = y * w;
    const zz = z * z;
    const zw = z * w;
    return [
        1 - 2 * (yy + zz), 2 * (xy - zw), 2 * (xz + yw), 0,
        2 * (xy + zw), 1 - 2 * (xx + zz), 2 * (yz - xw), 0,
        2 * (xz - yw), 2 * (yz + xw), 1 - 2 * (xx + yy), 0,
        0, 0, 0, 1
    ];
}
export function mat4x4mul(matrix1, matrix2) {
    const result = [
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 0
    ];
    for (var j = 0; j < 4; j++) {
        for (var i = 0; i < 4; i++) {
            var cellValue = 0;
            for (var k = 0; k < 4; k++) {
                cellValue += matrix1[4 * j + k] * matrix2[4 * k + i];
            }
            result[j * 4 + i] = cellValue;
        }
    }
    return result;
}
export function mat4x4perspectiveProjection(fovY, aspectRatio, zNear, zFar) {
    const f = Math.tan(0.5 * (Math.PI - fovY));
    const rangeInv = 1.0 / (zNear - zFar);
    return [
        f / aspectRatio, 0, 0, 0,
        0, f, 0, 0,
        0, 0, (zFar + zNear) / (zNear - zFar), 2 * zFar * zNear / (zNear - zFar),
        0, 0, -1, 0
    ];
}
export function mat4x4orthogonalProjection(left, right, bottom, top, near, far) {
    return [
        2 / (right - left), 0, 0, -(right + left) / (right - left),
        0, 2 / (top - bottom) / 2, 0, -(top + bottom) / (top - bottom),
        0, 0, -2 / (far - near), -(far + near) / (far - near),
        0, 0, 0, 1
    ];
}
export function mat4x4inv(matrix) {
    const result = [
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 0
    ];
    const m00 = matrix[0 * 4 + 0];
    const m01 = matrix[0 * 4 + 1];
    const m02 = matrix[0 * 4 + 2];
    const m03 = matrix[0 * 4 + 3];
    const m10 = matrix[1 * 4 + 0];
    const m11 = matrix[1 * 4 + 1];
    const m12 = matrix[1 * 4 + 2];
    const m13 = matrix[1 * 4 + 3];
    const m20 = matrix[2 * 4 + 0];
    const m21 = matrix[2 * 4 + 1];
    const m22 = matrix[2 * 4 + 2];
    const m23 = matrix[2 * 4 + 3];
    const m30 = matrix[3 * 4 + 0];
    const m31 = matrix[3 * 4 + 1];
    const m32 = matrix[3 * 4 + 2];
    const m33 = matrix[3 * 4 + 3];
    const tmp0 = m22 * m33;
    const tmp1 = m32 * m23;
    const tmp2 = m12 * m33;
    const tmp3 = m32 * m13;
    const tmp4 = m12 * m23;
    const tmp5 = m22 * m13;
    const tmp6 = m02 * m33;
    const tmp7 = m32 * m03;
    const tmp8 = m02 * m23;
    const tmp9 = m22 * m03;
    const tmp10 = m02 * m13;
    const tmp11 = m12 * m03;
    const tmp12 = m20 * m31;
    const tmp13 = m30 * m21;
    const tmp14 = m10 * m31;
    const tmp15 = m30 * m11;
    const tmp16 = m10 * m21;
    const tmp17 = m20 * m11;
    const tmp18 = m00 * m31;
    const tmp19 = m30 * m01;
    const tmp20 = m00 * m21;
    const tmp21 = m20 * m01;
    const tmp22 = m00 * m11;
    const tmp23 = m10 * m01;
    const t0 = (tmp0 * m11 + tmp3 * m21 + tmp4 * m31) - (tmp1 * m11 + tmp2 * m21 + tmp5 * m31);
    const t1 = (tmp1 * m01 + tmp6 * m21 + tmp9 * m31) - (tmp0 * m01 + tmp7 * m21 + tmp8 * m31);
    const t2 = (tmp2 * m01 + tmp7 * m11 + tmp10 * m31) - (tmp3 * m01 + tmp6 * m11 + tmp11 * m31);
    const t3 = (tmp5 * m01 + tmp8 * m11 + tmp11 * m21) - (tmp4 * m01 + tmp9 * m11 + tmp10 * m21);
    const d = 1 / (m00 * t0 + m10 * t1 + m20 * t2 + m30 * t3);
    return [
        d * t0,
        d * t1,
        d * t2,
        d * t3,
        d * ((tmp1 * m10 + tmp2 * m20 + tmp5 * m30) - (tmp0 * m10 + tmp3 * m20 + tmp4 * m30)),
        d * ((tmp0 * m00 + tmp7 * m20 + tmp8 * m30) - (tmp1 * m00 + tmp6 * m20 + tmp9 * m30)),
        d * ((tmp3 * m00 + tmp6 * m10 + tmp11 * m30) - (tmp2 * m00 + tmp7 * m10 + tmp10 * m30)),
        d * ((tmp4 * m00 + tmp9 * m10 + tmp10 * m20) - (tmp5 * m00 + tmp8 * m10 + tmp11 * m20)),
        d * ((tmp12 * m13 + tmp15 * m23 + tmp16 * m33) - (tmp13 * m13 + tmp14 * m23 + tmp17 * m33)),
        d * ((tmp13 * m03 + tmp18 * m23 + tmp21 * m33) - (tmp12 * m03 + tmp19 * m23 + tmp20 * m33)),
        d * ((tmp14 * m03 + tmp19 * m13 + tmp22 * m33) - (tmp15 * m03 + tmp18 * m13 + tmp23 * m33)),
        d * ((tmp17 * m03 + tmp20 * m13 + tmp23 * m23) - (tmp16 * m03 + tmp21 * m13 + tmp22 * m23)),
        d * ((tmp14 * m22 + tmp17 * m32 + tmp13 * m12) - (tmp16 * m32 + tmp12 * m12 + tmp15 * m22)),
        d * ((tmp20 * m32 + tmp12 * m02 + tmp19 * m22) - (tmp18 * m22 + tmp21 * m32 + tmp13 * m02)),
        d * ((tmp18 * m12 + tmp23 * m32 + tmp15 * m02) - (tmp22 * m32 + tmp14 * m02 + tmp19 * m12)),
        d * ((tmp22 * m22 + tmp16 * m02 + tmp21 * m12) - (tmp20 * m12 + tmp23 * m22 + tmp17 * m02))
    ];
}
