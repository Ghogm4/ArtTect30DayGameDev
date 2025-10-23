// ...existing code...
shader_type canvas_item;

uniform float startAngle = 0.0;    // 起始角度（度数，0 在 +X 方向）
uniform float currentAngle : hint_range(-90, 269, 1) = 0.0;  // 当前角度（度数），sector = currentAngle - startAngle (按度数计算，可超过360)
uniform float radius : hint_range(0.0, 1.0, 0.01) = 0.5; // 圆半径（UV 空间，0-0.5 常用）
uniform float edgeSoftness : hint_range(0.0, 0.2, 0.005) = 0.01; // 圆边缘平滑范围
uniform float angularSoftness : hint_range(0.0, 10.0, 0.5) = 1.0; // 角度边缘平滑（度）
uniform vec3 grayColor = vec3(0.6); // 冷却覆盖时的灰色

void fragment() {
    vec4 tex = texture(TEXTURE, UV);
    // 保留透明区域不处理（不能在 fragment 中使用 return）
    if (tex.a <= 0.001) {
        COLOR = tex;
    } else {

    vec2 center = vec2(0.5);
    vec2 d = UV - center;
    float dist = length(d);

    // 圆形遮罩（带软边）
    float inside = smoothstep(radius + edgeSoftness, radius - edgeSoftness, dist); // 1 inside, 0 outside

    // 计算像素角度（度数，范围 0..360)
    float ang = atan(d.y, d.x); // -PI..PI
    float angDeg = ang * 180.0 / PI;
    if (angDeg < 0.0) angDeg += 360.0;

    // 规范化起始和扫过角（度数）
    float start = mod(startAngle, 360.0);
    // sweep 为正向扫过量（0..360），允许 currentAngle < startAngle 表示环绕
    float sweep = mod(currentAngle - startAngle + 360.0, 360.0);

    // 计算像素相对于 start 的角度偏移（0..360）
    float rel = mod(angDeg - start + 360.0, 360.0);

    // 角度内的硬遮罩（rel <= sweep 表示在已露出的扇区内）
    float angularMask = step(rel, sweep);

    // 给角度边缘加平滑
    // 当 rel 接近 sweep 边界时过渡到 0
    float angSoft = max(0.001, angularSoftness);
    float angDist = rel - sweep;
    // 若 sweep 为 0（未露出），将 angMaskSoft 设置为 0
    float angMaskSoft = 0.0;
    if (sweep > 0.0001) {
        // 用 smoothstep 在 [ -angSoft, 0 ] 区间内平滑（负值表示在边界内）
        angMaskSoft = smoothstep(-angSoft, 0.0, -angDist);
    }

    float finalMask = inside * angMaskSoft;

    // 混合灰色与原色：在 finalMask == 1 时显示原色，0 时显示灰色*原色（保持纹理细节）
    vec3 shown = mix(grayColor * tex.rgb, tex.rgb, finalMask);

    COLOR = vec4(shown, tex.a);
}
}
// ...existing code...