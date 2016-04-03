Shader "Unlit/FogofWarMask"
{
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Threshold("Threshhold", Float) = 0.1
		_TransparentColor("Transparent Color", Color) = (1,1,1,1)
	}
		SubShader{
			Tags { "RenderType" = "Transparent" "LightMode" = "ForwardBase" }
			Blend SrcAlpha OneMinusSrcAlpha
			Lighting off
			LOD 200

			CGPROGRAM
			#pragma surface surf NoLighting noambient

			fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, float aten)
			{
				fixed4 color;
				color.rgb = s.Albedo;
				color.a = s.Alpha;
				return color;
			}

			fixed4 _Color;
			sampler2D _MainTex;

			struct Input {
				float2 uv_MainTex;
			};


			fixed4 _TransparentColor;
			half _Threshold;
		
			void surf (Input IN, inout SurfaceOutput o) {
				half4 baseColor = tex2D(_MainTex, IN.uv_MainTex);

				half3 transparent_diff = baseColor.xyz - _TransparentColor.xyz;
				half transparent_diff_squared = dot(transparent_diff, transparent_diff);

				if (transparent_diff_squared < _Threshold)
					discard;



				o.Albedo = _Color.rgb * baseColor.g;
				o.Alpha = _Color.a; //green - follor of apeture mask
			}
	ENDCG
	}
	FallBack "Diffuse"
}
