Shader "LevelShaders/Ambient" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_AmbientColor ("Ambient Color", Color) = (1,1,1,1)
		_EmissivColor ("Emissiv Color", Color) = (1,1,1,1)
		_Power("Power",Range(0,100)) = 5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 250
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		sampler2D _MainTex;
		struct Input {
			float2 uv_MainTex;
		};


		fixed4 _AmbientColor;
		fixed4 _Color;
		fixed4 _EmissivColor;
		float _Power;

		//UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		//UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * pow((_AmbientColor + _EmissivColor),_Power) * _Color ;
		 

			o.Albedo = c.rgb;

			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
