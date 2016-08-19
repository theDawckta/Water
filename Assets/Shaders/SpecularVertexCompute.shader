Shader "Tutorial/SpecularVertexCompute"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (1,1,1,1)
		_Shininess ("Shininess", Float) = 10	}

	SubShader
	{
		Pass 
		{
			Tags { "LightMode" = "ForwardBase" } 

			CGPROGRAM

			#pragma vertex vert             
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			StructuredBuffer<float3> buf_Points;
			StructuredBuffer<float3> buf_Normals;
			StructuredBuffer<float3> buf_Positions;

			uniform half4 _Color;
			uniform float4 _LightColor0;
			uniform float _Shininess;
			uniform float4 _SpecColor;

			struct vertOutput
			{
				float4 pos : SV_POSITION;                               
				half4 col : COLOR;                             
			};
			
			vertOutput vert(uint id : SV_VertexID, uint inst : SV_InstanceID)
			{
				vertOutput o;
				
				float4 normal = float4(buf_Normals[id], 0.0);
				float3 n = normalize(mul(normal, _World2Object));
				float3 l = normalize(_WorldSpaceLightPos0);
				float3 v = normalize(_WorldSpaceCameraPos);

				float NdotL = max(0.0, dot(n, l));
				float3 a = UNITY_LIGHTMODEL_AMBIENT * _Color;
				float3 d = NdotL * _LightColor0 * _Color;
				float3 r = reflect(-l, n);
				float RdotV = max(0.0, dot(r, v));
				float3 s = float3(0,0,0);
				if (dot(n, l) > 0.0) 
				s = _LightColor0 * _SpecColor * pow(RdotV, _Shininess);
 
                float4 c = float4(d+a+s, 1.0);
                o.col = c;
				float3 worldPos = buf_Points[id] + buf_Positions[inst];
				o.pos = mul (UNITY_MATRIX_VP, float4(worldPos,1.0f));
				//o.pos = mul(UNITY_MATRIX_MVP, buf_Positions[inst]);
				return o;
			}

			half4 frag(vertOutput input) : COLOR
			{        
				return saturate(input.col); 
			}

		ENDCG
		}
	}
}