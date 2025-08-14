shader_type canvas_item;
uniform float x_cordinates= 0.4694;
uniform float x_width=0.006;
uniform float y_cordinates=0.3;
uniform float y_width=0.01;
uniform float intesity=0.2;
uniform float speed=0.2;
uniform float limit_y=0.8;
void fragment()
{
    vec4 texture_color=texture(TEXTURE,UV);
    float moving_y=mod(TIME*speed,limit_y);
    float dis = distance(UV,vec2(x_cordinates,min(y_cordinates+moving_y,limit_y)));
    float coeff=max(1.0,dis/(1.414*length(vec2(x_width,y_width)*1.5)));
    float moving_y1=mod((TIME-5.0)*speed,limit_y);
    float dis1 = distance(UV,vec2(x_cordinates,min(y_cordinates+moving_y1,limit_y)));
    float coeff1=max(1.0,dis1/(1.414*length(vec2(x_width,y_width)*1.2)));
    float moving_y2=mod((TIME-10.0)*speed,limit_y);
    float dis2 = distance(UV,vec2(x_cordinates,min(y_cordinates+moving_y2,limit_y)));
    float coeff2=max(1.0,dis2/(1.414*length(vec2(x_width,y_width)*1.0)));
    if (UV.y<=limit_y && coeff<=coeff1 && coeff<=coeff2)
    {
        COLOR = min(texture_color+vec4(intesity)/coeff,vec4(1.0));
    }else if(UV.y<=limit_y && coeff>coeff1 && coeff1<=coeff2)
    {
        COLOR = min(texture_color+vec4(intesity)/coeff1,vec4(1.0));
    }else if(UV.y<=limit_y && coeff1>coeff2)
    {
        COLOR = min(texture_color+vec4(intesity)/coeff2,vec4(1.0));
    }else
    {
        COLOR = texture_color;
    }
}