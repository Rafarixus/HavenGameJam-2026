Shader "Haven/Tut1 Twilight Sky"
{
 Properties { _Exposure("Exposure",Float)=1 }
 SubShader
 {
  Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
  Cull Off ZWrite Off
  Pass
  {
   HLSLPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
   struct Attributes { float4 positionOS:POSITION; };
   struct Varyings { float4 positionCS:SV_POSITION;float3 direction:TEXCOORD0; };
   float _Exposure;
   Varyings vert(Attributes i){Varyings o;o.positionCS=TransformObjectToHClip(i.positionOS.xyz);o.direction=i.positionOS.xyz;return o;}
   half4 frag(Varyings i):SV_Target
   {
    float3 d=normalize(i.direction);float h=saturate(d.y);
    float3 horizon=float3(.31,.43,.5),zenith=float3(.035,.085,.17);
    float3 col=lerp(horizon,zenith,pow(h,.42));
    float3 sun=normalize(float3(.8,.32,.45));float s=dot(d,sun);
    col+=float3(.55,.24,.09)*pow(saturate(s),16)*.55;
    col+=float3(1,.65,.32)*smoothstep(.9992,.9995,s)*3;
    col=lerp(float3(.12,.2,.27),col,smoothstep(-.15,.05,d.y));
    return half4(col*_Exposure,1);
   }
   ENDHLSL
  }
 }
}
