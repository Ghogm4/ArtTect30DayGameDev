shader_type canvas_item;
uniform float darkness : hint_range(0.0, 1.0, 0.05) = 1.0;
void fragment() {
	vec4 color = texture(TEXTURE, UV);
	COLOR = vec4(color.rgb * darkness, color.a);
}
