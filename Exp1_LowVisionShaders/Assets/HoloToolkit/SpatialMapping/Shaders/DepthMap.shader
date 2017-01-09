Shader "Custom/DepthMap"
{
	Properties{
		_CameraPos("_CameraPos", Vector) = (0,0,0)	// Camera position from main camera
		_Equation("_Equation", int) = 0				// Specifies depth mapping strategy (from Voice Placement)
		_Color("_Color", int) = 0					// Specifies color to use (from Voice Placement)
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		// Values must be declared outside of property block as well
		float3 _CameraPos;
	int _Equation;
	int _Color;

	// settings for Zebra shader and set them to false
	int zebraSwitch = 0;

	// This is the data structure that the vertex program provides to the fragment
	struct v2f
	{
		float4 viewPos : SV_POSITION;
		float3 normal : NORMAL;
		float4 worldPos: TEXCOORD0;
	};

	// Returns the position of a vertex
	v2f vert(appdata_base v)
	{
		v2f o;

		// Calculate where the vertex is in view space.
		o.viewPos = mul(UNITY_MATRIX_MVP, v.vertex);

		// Calculate the normal in WorldSpace.
		o.normal = UnityObjectToWorldNormal(v.normal);

		// Calculate where the object is in world space.
		o.worldPos = mul(unity_ObjectToWorld, v.vertex);

		return o;
	}

	// Sets the color of a fragment based on the distance and strategy/color inputs
	fixed4 frag(v2f i) : SV_Target
	{
		// Declare return value and initialize RGBA to 0,0,0,1 (so we don't have to set every time below)
		fixed4 ret;
	ret.r = 0; ret.g = 0; ret.b = 0; ret.a = 1;

	// Generate the distance value
	float x_dist = _CameraPos.x - i.worldPos.x;
	float x_sqrd = x_dist * x_dist;
	float y_dist = _CameraPos.y - i.worldPos.y;
	float y_sqrd = y_dist * y_dist;
	float z_dist = _CameraPos.z - i.worldPos.z;
	float z_sqrd = z_dist * z_dist;
	float total = x_sqrd + y_sqrd + z_sqrd;
	float distance = sqrt(total);

	// Set the depth depending on strategy input from user
	float depth = 0;
	if (distance <= 0.5) {					// Any distance within arm's length will have the
		depth = 1;							// max depth value regardless

											// Standard strategy - calculate depth based on standardized distance from user
	}
	else if (_Equation == 0) {
		depth = distance / 15;

		// Inverted strategy - calculate depth based on standardized inverted distance from user
	}
	else if (_Equation == 1) {
		depth = 1 - distance / 15;

		// Scaled strategy - calculate depth based on scaled inverted distance from user
	}
	else if (_Equation == 2) {
		depth = 1 - distance / 5;

		// Non-linear strategy - calculate depth based on scaled non-linear inverted distance from user
	}
	else if (_Equation == 3) {
		depth = ((1 / distance) - (1 / 5)) / ((1 / 0.5) - (1 / 5));

		// Color-code stragegy - color codes specific distances from the user: 4 different colors
	}

	else if (_Equation == 4) {
		if (distance <= 1.5) {							    // 0 - 1.5 meters = red
			ret.r = 1;
		}
		else if ((distance > 1.5) && (distance <= 3.5)) {		// 1.5 - 2.5 meters = orange
			ret.r = 1; ret.g = 0.5;
		}
		else if ((distance > 3.5) && (distance <= 5.5)) {		// 2.5 - 3.5 meters = yellow
			ret.r = 1; ret.g = 1;
		}
		else {							// 5.5+ meters = dark grey
			ret.r = 0.1; ret.g = 0.1; ret.b = 0.1;
		}

		return ret;	    // Need to return frag here with color code strategy so color isn't changed below


	}
	// same as before but 5 different colors
	else if (_Equation == 5) {
		if (distance <= 1.5) {							    // 0 - 1.5 meters = red
			ret.r = 1; ret.a = 0.5;
		}
		else if ((distance > 1.5) && (distance <= 2.84)) {		// 1.5 - 2.5 meters = orange
			ret.r = 1; ret.g = 0.5; ret.a = 0.6;
		}
		else if ((distance > 2.84) && (distance <= 4.17)) {		// 2.5 - 3.5 meters = yellow
			ret.r = 1; ret.g = 1; ret.a = 0.7;
		}
		else if ((distance > 4.17) && (distance <= 5.5)) {		// 3.5 - 4.5 meters = green
			ret.g = 1; ret.a = 0.8;
		}
		else {							// 5.5+ meters
			ret.b = 1; ret.a = 0.9;
		}

		ret.r *= ret.a;
		ret.g *= ret.a;
		ret.b *= ret.a;
		return ret;	    // Need to return frag here with color code strategy so color isn't changed below


	}
	// same as before but 6 colors
	else if (_Equation == 6) {
		if (distance <= 1.5) {							    // 0 - 1.5 meters = red
			ret.r = 1; ret.a = 0.5;
		}
		else if ((distance > 1.5) && (distance <= 2.5)) {		// 1.5 - 2.5 meters = orange
			ret.r = 1; ret.g = 0.5; ret.a = 0.6;
		}
		else if ((distance > 2.5) && (distance <= 3.5)) {		// 2.5 - 3.5 meters = yellow
			ret.r = 1; ret.g = 1; ret.a = 0.7;
		}
		else if ((distance > 3.5) && (distance <= 4.5)) {		// 3.5 - 4.5 meters = green
			ret.g = 1; ret.a = 0.8;
		}
		else if ((distance > 4.5) && (distance <= 5.5)) {		// 4.5 - 5.5 meters = blue
			ret.b = 1; ret.a = 0.9;
		}
		else {							// 5.5+ meters = purple
			ret.r = 0.63; ret.g = 0.13; ret.b = 0.95; ret.a = 1;
		}

		ret.r *= ret.a;
		ret.g *= ret.a;
		ret.b *= ret.a;
		return ret;	    // Need to return frag here with color code strategy so color isn't changed below


	}
	// dynamic shader 1: Flickering stripes at 1Hz			
	else if (_Equation == 7) {

		// try this 
		/*
		if (distance + _Time.y  % 15 < 3) {

		}*/


		if (_Time.y % 2 >= 0 && _Time.y % 2 < 1) {
			//ret.g = 1; ret.a = 1.0;
			if (distance <= 0.5) {							    // 0 - 0.5 meters	= white
				ret.g = 1; ret.a = 1.0;
			}
			else if ((distance > 0.5) && (distance <= 1.0)) {		// 1.5 - 2.5 meters = dark grey
				ret.g = 1; ret.a = 0.5;
			}
			else if ((distance > 1.0) && (distance <= 1.5)) {		// 2.5 - 3.5 meters = white
				ret.g = 1; ret.a = 1.0;
			}
			else if ((distance > 1.5) && (distance <= 2)) {		// 3.5 - 4.5 meters = dark grey
				ret.g = 1; ret.a = 0.5;
			}
			else if ((distance > 2.0) && (distance <= 2.5)) {		// 4.5 - 5.5 meters = white
				ret.g = 1; ret.a = 1.0;
			}
			else if ((distance > 2.5) && (distance <= 3.0)) {		// 3.5 - 4.5 meters = dark grey
				ret.g = 1; ret.a = 0.5;
			}
			else if ((distance > 3.0) && (distance <= 3.5)) {		// 4.5 - 5.5 meters = white
				ret.g = 1; ret.a = 1.0;
			}
			else if ((distance > 3.5) && (distance <= 4.0)) {		// 3.5 - 4.5 meters = dark grey
				ret.g = 1; ret.a = 0.5;
			}
			else if ((distance > 4.0) && (distance <= 4.5)) {		// 4.5 - 5.5 meters = white
				ret.g = 1; ret.a = 1.0;
			}
			else if ((distance > 4.5) && (distance <= 5.0)) {		// 3.5 - 4.5 meters = dark grey
				ret.g = 1; ret.a = 0.5;
			}
			else if ((distance > 5.0) && (distance <= 5.5)) {		// 4.5 - 5.5 meters = white
				ret.g = 1; ret.a = 1.0;
			}
			else {							// 5.5+ meters = dark grey
				ret.g = 1; ret.a = 0.5;
			}
		}
		else if (_Time.y % 2 >= 1 && _Time.y % 2< 2) {
			//ret.r = 0; ret.g = 1; ret.b = 1;  ret.a = 0.5;
			if (distance <= 0.5) {							    // 0 - 0.5 meters	= dark grey
				ret.g = 1; ret.a = 0.5;
			}
			else if ((distance > 0.5) && (distance <= 1.0)) {		// 1.5 - 2.5 meters = white
				ret.g = 1; ret.a = 1.0;
			}
			else if ((distance > 1.0) && (distance <= 1.5)) {		// 2.5 - 3.5 meters = dark grey
				ret.g = 1; ret.a = 0.5;
			}
			else if ((distance > 1.5) && (distance <= 2)) {		// 3.5 - 4.5 meters  = white

				ret.g = 1; ret.a = 1.0;
			}
			else if ((distance > 2.0) && (distance <= 2.5)) {		// 4.5 - 5.5 meters = dark grey
				ret.g = 1; ret.a = 0.5;
			}
			else if ((distance > 2.5) && (distance <= 3.0)) {		// 3.5 - 4.5 meters = white

				ret.g = 1; ret.a = 1.0;
			}
			else if ((distance > 3.0) && (distance <= 3.5)) {		// 4.5 - 5.5 meters = dark grey
				ret.g = 1; ret.a = 0.5;
			}
			else if ((distance > 3.5) && (distance <= 4.0)) {		// 3.5 - 4.5 meters = white

				ret.g = 1; ret.a = 1.0;
			}
			else if ((distance > 4.0) && (distance <= 4.5)) {		// 4.5 - 5.5 meters
				ret.g = 1; ret.a = 0.5;
			}
			else if ((distance > 4.5) && (distance <= 5.0)) {		// 3.5 - 4.5 meters = dark grey

				ret.g = 1; ret.a = 1.0;
			}
			else if ((distance > 5.0) && (distance <= 5.5)) {		// 4.5 - 5.5 meters = white
				ret.g = 1; ret.a = 0.5;
			}
			else {							// 5.5+ meters = dark grey
				ret.g = 1; ret.a = 1.0;
			}
		}

		ret.r *= ret.a;
		ret.g *= ret.a;
		ret.b *= ret.a;

		return ret;	    // Need to return frag here with color code strategy so color isn't changed below

	}
	// Dynamic shader 2: Flicker rate changes over distance (the closer the higher the flicker rate
	else if (_Equation == 8) {

		if (distance <= 1.5) {											// 1 - 1.5 meters	= white
			if (_Time.y % 0.1 >= 0 && _Time.y % 0.1 < 0.05) {			// 6Hz flicker
				ret.g = 1; ret.a = 0.5;									// red
			}
			else if (_Time.y % 0.1 >= 0.05 && _Time.y % 0.1 < 0.1) {
				ret.g = 1; ret.a = 1;									// blue
			}
		}
		else if ((distance > 1.5) && (distance <= 2.5)) {			// 2.5 - 2.5 meters
			if (_Time.y % 0.2 >= 0 && _Time.y % 0.2 < 0.1) {		// 5.5Hz flicker
				ret.g = 1; ret.a = 1;								// blue  (red and blue grey alternate)
			}
			else if (_Time.y % 0.2 >= 0.1 && _Time.y % 0.2 < 0.2) {
				ret.g = 1; ret.a = 0.5;								 // red
			}
		}
		else if ((distance > 2.5) && (distance <= 3.5)) {		// 2.5 - 3.5 meters
			if (_Time.y % 0.3 >= 0 && _Time.y % 0.3 < 0.15) {	// 5Hz flicker
				ret.g = 1; ret.a = 0.5;
			}
			else if (_Time.y % 0.3 >= 0.15 && _Time.y % 0.3 < 0.3) {
				ret.g = 1; ret.a = 1;
			}
		}
		else if ((distance > 3.5) && (distance <= 4.5)) {					// 3.5 - 4.5 m
			if (_Time.y % 0.4 >= 0 && _Time.y % 0.4 < 0.2) {		// 4.5Hz flicker
				ret.g = 1; ret.a = 1;
			}
			else if (_Time.y % 0.4 >= 0.2 && _Time.y % 0.4 < 0.4) {
				ret.g = 1; ret.a = 0.5;
			}
		}
		else if ((distance > 4.5) && (distance <= 5.5)) {					// 4.5 - 5.5 m
			if (_Time.y % 0.5 >= 0 && _Time.y % 0.5 < 0.25) {		// 4.5Hz flicker
				ret.g = 1; ret.a = 0.5;
			}
			else if (_Time.y % 0.5 >= 0.25 && _Time.y % 0.5 < 0.5) {
				ret.g = 1; ret.a = 1;
			}
		}
		else {							// 5.5+ meters = dark grey
			ret.g = 1; ret.a = 0.5;
		}


		ret.r *= ret.a;
		ret.g *= ret.a;
		ret.b *= ret.a;

		return ret;	    // Need to return frag here with color code strategy so color isn't changed below

	}
	// distance shader but all one color (alpha step): same strategy as equation 1-4 but discretized
	else if (_Equation == 9) {
		if (distance <= 1.5) {							    // 0 - 1.5 meters
			ret.g = 1; ret.a = 1.0;
		}
		else if ((distance > 1.5) && (distance <= 2.5)) {		// 1.5 - 2.5 meters
			ret.g = 1; ret.a = 0.8;
		}
		else if ((distance > 2.5) && (distance <= 3.5)) {		// 2.5 - 3.5 meters
			ret.g = 1; ret.a = 0.6;
		}
		else if ((distance > 3.5) && (distance <= 4.5)) {		// 3.5 - 4.5 meters
			ret.g = 1; ret.a = 0.4;
		}
		else if ((distance > 4.5) && (distance <= 5.5)) {		// 4.5 - 5.5 meters
			ret.g = 1; ret.a = 0.2;
		}
		else {							// 5.5+ meters
			ret.g = 1; ret.a = 0.1;
		}

		ret.r *= ret.a;
		ret.g *= ret.a;
		ret.b *= ret.a;
		return ret;	    // Need to return frag here with color code strategy so color isn't changed below


	}
	// Dynamic shader 3: pulsating instead of flickering (alpha from 0.0 - 0.5)
	else if (_Equation == 10) {

		float curTime = _Time.y;

		if (distance <= 1.5) {											// 1 - 1.5 meters	= white
			if (_Time.y % 0.1 >= 0 && _Time.y % 0.1 < 0.05) {			// 
				ret.g = 1;
				ret.a = ((curTime % 0.1 - 0) / 0.1 - 0);  // have value change between zero and 1
			}
			else if (_Time.y % 0.1 >= 0.05 && _Time.y % 0.1 < 0.1) {
				ret.g = 1;
				ret.a = 1 - ((curTime % 0.1 - 0) / 0.1 - 0);									// blue
			}
		}
		else if ((distance > 1.5) && (distance <= 2.5)) {			// 2.5 - 2.5 meters
			if (_Time.y % 0.2 >= 0 && _Time.y % 0.2 < 0.1) {		//
				ret.g = 1;
				ret.a = ((curTime % 0.2 - 0) / 0.2 - 0);
			}
			else if (_Time.y % 0.2 >= 0.1 && _Time.y % 0.2 < 0.2) {
				ret.g = 1;
				ret.a = 1 - ((curTime % 0.2 - 0) / 0.2 - 0);
			}
		}
		else if ((distance > 2.5) && (distance <= 3.5)) {		// 2.5 - 3.5 meters
			if (_Time.y % 0.3 >= 0 && _Time.y % 0.3 < 0.15) {	// 
				ret.g = 1;
				ret.a = ((curTime % 0.3 - 0) / 0.3 - 0);
			}
			else if (_Time.y % 0.3 >= 0.15 && _Time.y % 0.3 < 0.3) {
				ret.g = 1;
				ret.a = 1 - ((curTime % 0.3 - 0) / 0.3 - 0);
			}
		}
		else if ((distance > 3.5) && (distance <= 4.5)) {					// 3.5 - 4.5 m
			if (_Time.y % 0.4 >= 0 && _Time.y % 0.4 < 0.2) {		// 4.5Hz flicker
				ret.g = 1;
				ret.a = ((curTime % 0.4 - 0) / 0.4 - 0);
			}
			else if (_Time.y % 0.4 >= 0.2 && _Time.y % 0.4 < 0.4) {
				ret.g = 1;
				ret.a = 1 - ((curTime % 0.4 - 0) / 0.4 - 0);
			}
		}
		else if ((distance > 4.5) && (distance <= 5.5)) {					// 4.5 - 5.5 m
			if (_Time.y % 0.5 >= 0 && _Time.y % 0.5 < 0.25) {		// 4.5Hz flicker
				ret.g = 1;
				ret.a = ((curTime % 0.5 - 0) / 0.5 - 0);
			}
			else if (_Time.y % 0.5 >= 0.25 && _Time.y % 0.5 < 0.5) {
				ret.g = 1;
				ret.a = 1 - ((curTime % 0.5 - 0) / 0.5 - 0);
			}
		}
		else {							// 5.5+ meters = dark grey
			ret.g = 1;
			ret.a = 0.1;
		}



		ret.r *= ret.a;
		ret.g *= ret.a;
		ret.b *= ret.a;

		return ret;	    // Need to return frag here with color code strategy so color isn't changed below

	}
	// Dynamic shader 3: pulsating; same as before but higher brightness (alpha from 0.5 - 1)
	else if (_Equation == 11) {

		float curTime = _Time.y;

		if (distance <= 1.5) {											// 1 - 1.5 meters	= white
			if (_Time.y % 0.1 >= 0 && _Time.y % 0.1 < 0.05) {			// 
				ret.g = 1;
				ret.a = 0.5 + ((curTime % 0.1 - 0) / 0.1 - 0);  // have value change between zero and 1
			}
			else if (_Time.y % 0.1 >= 0.05 && _Time.y % 0.1 < 0.1) {
				ret.g = 1;
				ret.a = 1.5 - ((curTime % 0.1 - 0) / 0.1 - 0);									// blue
			}
		}
		else if ((distance > 1.5) && (distance <= 2.5)) {			// 2.5 - 2.5 meters
			if (_Time.y % 0.2 >= 0 && _Time.y % 0.2 < 0.1) {		//
				ret.g = 1;
				ret.a = 0.5 + ((curTime % 0.2 - 0) / 0.2 - 0);
			}
			else if (_Time.y % 0.2 >= 0.1 && _Time.y % 0.2 < 0.2) {
				ret.g = 1;
				ret.a = 1.5 - ((curTime % 0.2 - 0) / 0.2 - 0);
			}
		}
		else if ((distance > 2.5) && (distance <= 3.5)) {		// 2.5 - 3.5 meters
			if (_Time.y % 0.3 >= 0 && _Time.y % 0.3 < 0.15) {	// 
				ret.g = 1;
				ret.a = 0.5 + ((curTime % 0.3 - 0) / 0.3 - 0);
			}
			else if (_Time.y % 0.3 >= 0.15 && _Time.y % 0.3 < 0.3) {
				ret.g = 1;
				ret.a = 1.5 - ((curTime % 0.3 - 0) / 0.3 - 0);
			}
		}
		else if ((distance > 3.5) && (distance <= 4.5)) {					// 3.5 - 4.5 m
			if (_Time.y % 0.4 >= 0 && _Time.y % 0.4 < 0.2) {		// 4.5Hz flicker
				ret.g = 1;
				ret.a = 0.5 + ((curTime % 0.4 - 0) / 0.4 - 0);
			}
			else if (_Time.y % 0.4 >= 0.2 && _Time.y % 0.4 < 0.4) {
				ret.g = 1;
				ret.a = 1.5 - ((curTime % 0.4 - 0) / 0.4 - 0);
			}
		}
		else if ((distance > 4.5) && (distance <= 5.5)) {					// 4.5 - 5.5 m
			if (_Time.y % 0.5 >= 0 && _Time.y % 0.5 < 0.25) {		// 4.5Hz flicker
				ret.g = 1;
				ret.a = 0.5 + ((curTime % 0.5 - 0) / 0.5 - 0);
			}
			else if (_Time.y % 0.5 >= 0.25 && _Time.y % 0.5 < 0.5) {
				ret.g = 1;
				ret.a = 1.5 - ((curTime % 0.5 - 0) / 0.5 - 0);
			}
		}
		else {							// 5.5+ meters = dark grey
			ret.g = 1;
			ret.a = 0.1;
		}



		ret.r *= ret.a;
		ret.g *= ret.a;
		ret.b *= ret.a;

		return ret;	    // Need to return frag here with color code strategy so color isn't changed below

	}

	///////////////////
	///////////////////
	else if (_Equation == 12) {
		if (distance <= 1.5) {
			ret.r = 215 / 255.0f;
			ret.g = 48 / 255.0f;
			ret.b = 39 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 1.5) && (distance <= 2.5)) {
			ret.r = (252 / 255.0f);
			ret.g = (141 / 255.0f);
			ret.b = (49 / 255.0f);
			ret.a = 1.0;
		}
		else if ((distance > 2.5) && (distance <= 3.5)) {
			ret.r = 254 / 255.0f;
			ret.g = 224 / 255.0f;
			ret.b = 144 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 3.5) && (distance <= 4.5)) {
			ret.r = 224 / 255.0f;
			ret.g = 243 / 255.0f;
			ret.b = 248 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 4.5) && (distance <= 5.5)) {
			ret.r = 145 / 255.0f;
			ret.g = 191 / 255.0f;
			ret.b = 219 / 255.0f;
			ret.a = 1.0;
		}
		else {
			ret.r = 69 / 255.0f;
			ret.g = 117 / 255.0f;
			ret.b = 180 / 255.0f;
			ret.a = 1.0;
		}

		ret.r *= ret.a;
		ret.g *= ret.a;
		ret.b *= ret.a;
		return ret;
	}


	///////////////////
	///////////////////
	else if (_Equation == 13) {
		if (distance <= 1.5) {
			ret.r = 215 / 255.0f;
			ret.g = 25 / 255.0f;
			ret.b = 28 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 1.5) && (distance <= 2.5)) {
			ret.r = 253 / 255.0f;
			ret.g = 174 / 255.0f;
			ret.b = 97 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 2.5) && (distance <= 3.5)) {
			ret.r = 255 / 255.0f;
			ret.g = 255 / 255.0f;
			ret.b = 191 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 3.5) && (distance <= 4.5)) {
			ret.r = 171 / 255.0f;
			ret.g = 217 / 255.0f;
			ret.b = 233 / 255.0f;
			ret.a = 1.0;
		}
		else {
			ret.r = 44 / 255.0f;
			ret.g = 123 / 255.0f;
			ret.b = 182 / 255.0f;
			ret.a = 1.0;
		}

		ret.r *= ret.a;
		ret.g *= ret.a;
		ret.b *= ret.a;
		return ret;
	}

	///////////////////
	///////////////////
	else if (_Equation == 14) {
		if (distance <= 1.5) {
			ret.r = 255 / 255.0f;
			ret.g = 255 / 255.0f;
			ret.b = 204 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 1.5) && (distance <= 2.5)) {
			ret.r = (199 / 255.0f);
			ret.g = (233 / 255.0f);
			ret.b = (180 / 255.0f);
			ret.a = 1.0;
		}
		else if ((distance > 2.5) && (distance <= 3.5)) {
			ret.r = 127 / 255.0f;
			ret.g = 205 / 255.0f;
			ret.b = 187 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 3.5) && (distance <= 4.5)) {
			ret.r = 65 / 255.0f;
			ret.g = 182 / 255.0f;
			ret.b = 196 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 4.5) && (distance <= 5.5)) {
			ret.r = 44 / 255.0f;
			ret.g = 127 / 255.0f;
			ret.b = 184 / 255.0f;
			ret.a = 1.0;
		}
		else {
			ret.r = 37 / 255.0f;
			ret.g = 52 / 255.0f;
			ret.b = 148 / 255.0f;
			ret.a = 1.0;
		}

		ret.r *= ret.a;
		ret.g *= ret.a;
		ret.b *= ret.a;
		return ret;
	}

	///////////////////
	///////////////////
	else if (_Equation == 15) {
		if (distance <= 1.5) {
			ret.r = 255 / 255.0f;
			ret.g = 255 / 255.0f;
			ret.b = 204 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 1.5) && (distance <= 2.5)) {
			ret.r = 161 / 255.0f;
			ret.g = 218 / 255.0f;
			ret.b = 180 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 2.5) && (distance <= 3.5)) {
			ret.r = 65 / 255.0f;
			ret.g = 182 / 255.0f;
			ret.b = 196 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 3.5) && (distance <= 4.5)) {
			ret.r = 44 / 255.0f;
			ret.g = 127 / 255.0f;
			ret.b = 184 / 255.0f;
			ret.a = 1.0;
		}
		else {
			ret.r = 37 / 255.0f;
			ret.g = 52 / 255.0f;
			ret.b = 148 / 255.0f;
			ret.a = 1.0;
		}

		ret.r *= ret.a;
		ret.g *= ret.a;
		ret.b *= ret.a;
		return ret;
	}

	///////////////////
	///////////////////
	else if (_Equation == 16) {
		if (distance <= 1.5) {
			ret.r = 255 / 255.0f;
			ret.g = 255 / 255.0f;
			ret.b = 178 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 1.5) && (distance <= 2.5)) {
			ret.r = 254 / 255.0f;
			ret.g = 217 / 255.0f;
			ret.b = 118 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 2.5) && (distance <= 3.5)) {
			ret.r = 254 / 255.0f;
			ret.g = 178 / 255.0f;
			ret.b = 76 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 3.5) && (distance <= 4.5)) {
			ret.r = 253 / 255.0f;
			ret.g = 141 / 255.0f;
			ret.b = 60 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 4.5) && (distance <= 5.5)) {
			ret.r = 240 / 255.0f;
			ret.g = 59 / 255.0f;
			ret.b = 32 / 255.0f;
			ret.a = 1.0;
		}
		else {
			ret.r = 189 / 255.0f;
			ret.g = 0 / 255.0f;
			ret.b = 38 / 255.0f;
			ret.a = 1.0;
		}

		ret.r *= ret.a;
		ret.g *= ret.a;
		ret.b *= ret.a;
		return ret;
	}

	///////////////////
	///////////////////
	else if (_Equation == 17) {
		if (distance <= 1.5) {
			ret.r = 255 / 255.0f;
			ret.g = 255 / 255.0f;
			ret.b = 178 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 1.5) && (distance <= 2.5)) {
			ret.r = 254 / 255.0f;
			ret.g = 204 / 255.0f;
			ret.b = 92 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 2.5) && (distance <= 3.5)) {
			ret.r = 253 / 255.0f;
			ret.g = 141 / 255.0f;
			ret.b = 60 / 255.0f;
			ret.a = 1.0;
		}
		else if ((distance > 3.5) && (distance <= 4.5)) {
			ret.r = 240 / 255.0f;
			ret.g = 59 / 255.0f;
			ret.b = 32 / 255.0f;
			ret.a = 1.0;
		}
		else {
			ret.r = 189 / 255.0f;
			ret.g = 0 / 255.0f;
			ret.b = 38 / 255.0f;
			ret.a = 1.0;
		}

		ret.r *= ret.a;
		ret.g *= ret.a;
		ret.b *= ret.a;
		return ret;
	}
	///////////////////
	// HoloLens asjustment overlay: everything white
	else if (_Equation == 18) {

		ret.r = 1;
		ret.g = 1;
		ret.b = 1;
		ret.a = 1;

		ret.r *= ret.a;
		ret.g *= ret.a;
		ret.b *= ret.a;
		return ret;	    // Need to return frag here with color code strategy so color isn't changed below

	}// Standard strategy - calculate depth based on standardized distance from user
	/*else if (_Equation == 19) {
		ret.r = 1 / distance;
		ret.g = 0;
		ret.b = distance;
		ret.a = 1;

		ret.r *= ret.a;
		ret.g *= ret.a;
		ret.b *= ret.a;
		return ret;
	}*/
	// Set the color depending on input from the user
	if (_Color == 0) {						    // White
		ret.r = depth; ret.g = depth; ret.b = depth;
	}
	else if (_Color == 1) {				    // Blue
		ret.r = depth;
	}
	else if (_Color == 2) {				    // Green
		ret.g = depth;
	}
	else if (_Color == 3) {					// Red
		ret.b = depth;
	}
	else if (_Color == 4) {					// Yellow
		ret.r = depth; ret.g = depth;
	}
	else if (_Color == 5) {					// Orange
		ret.r = depth; ret.g = depth / 0.65;
	}
	else if (_Color == 6) {					// Purple
		ret.r = depth / 0.65; ret.g = depth / 0.13; ret.b = depth / 0.95;
	}
	else {									// Grey
		ret.r = depth / 0.5; ret.g = depth / 0.5; ret.b = depth / 0.5;
	}

	// Return the fragment containing depth info
	return ret;
	}
		ENDCG
	}
	}
}