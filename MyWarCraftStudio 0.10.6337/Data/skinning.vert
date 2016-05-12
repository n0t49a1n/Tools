attribute vec4 bone;
attribute vec4 weight;
uniform mat4 boneMatrices[60];

void main()
{	
    vec4 blendVertex;
    vec4 blendWeight = weight;
	ivec4 blendBone = ivec4(bone);
    
    for(int i = 0; i < 4; i++)
    {	
    	if(blendWeight.x > 0.0)
    	{	
		 	blendVertex += vec4((boneMatrices[blendBone.x] * gl_Vertex).xyz, 1.0) * blendWeight.x;
     	 	blendBone = blendBone.yzwx;
    		blendWeight = blendWeight.yzwx;
   		}
		else break;
    }
	
   	gl_Position = gl_ModelViewProjectionMatrix * blendVertex;
	gl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord0;
	gl_FrontColor = gl_Color;
	
	vec4 ecPos = gl_ModelViewMatrix * gl_Vertex;
	gl_FogFragCoord = abs(ecPos.z);
}