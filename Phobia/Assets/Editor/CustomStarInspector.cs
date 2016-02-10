using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

// A custom inspector for the shader that looks like the
//      ps3 main menu. Based on the shadertoy shader
public class CustomStarInspector : MaterialEditor
{
    public override void OnInspectorGUI()
    {
        // if we are not visible... return
        if (!isVisible)
            return;

        Material targetMat = target as Material;
        EditorGUI.BeginChangeCheck();
        
        //
        float speed = targetMat.GetFloat("speed");

        float iterations = targetMat.GetFloat("iterations");
        float formuparam = targetMat.GetFloat("formuparam");
        float volsteps = targetMat.GetFloat("volsteps");
        float stepsize = targetMat.GetFloat("stepsize");
        float zoom = targetMat.GetFloat("zoom");
        float tile = targetMat.GetFloat("tile");
        float brightness = targetMat.GetFloat("brightness");
        float darkmatter = targetMat.GetFloat("darkmatter");
        float distfading = targetMat.GetFloat("distfading");
        float saturation = targetMat.GetFloat("saturation");

        speed = EditorGUILayout.Slider("Speed", speed, 0.0f, 1.0f);

        float half; Rect a;

        a = EditorGUILayout.GetControlRect();
        half = a.width * 0.5f;

        iterations = EditorGUI.FloatField(new Rect(a.x, a.y, half, a.height), "Iterations", iterations);
        formuparam = EditorGUI.FloatField(new Rect(a.x + half, a.y, half, a.height), "FormUParam", formuparam);
        a = EditorGUILayout.GetControlRect();
        volsteps = EditorGUI.FloatField(new Rect(a.x, a.y, half, a.height), "VolSteps", volsteps);
        stepsize = EditorGUI.FloatField(new Rect(a.x + half, a.y, half, a.height), "StepSize", stepsize);
        a = EditorGUILayout.GetControlRect();
        zoom = EditorGUI.FloatField(new Rect(a.x, a.y, half, a.height), "Zoom", zoom);
        tile = EditorGUI.FloatField(new Rect(a.x + half, a.y, half, a.height), "Tile", tile);
        a = EditorGUILayout.GetControlRect();
        brightness = EditorGUI.FloatField(new Rect(a.x, a.y, half, a.height), "Brightness", brightness);
        darkmatter = EditorGUI.FloatField(new Rect(a.x + half, a.y, half, a.height), "DarkMatter", darkmatter);
        a = EditorGUILayout.GetControlRect();
        distfading = EditorGUI.FloatField(new Rect(a.x, a.y, half, a.height), "DistFading", distfading);
        saturation = EditorGUI.FloatField(new Rect(a.x + half, a.y, half, a.height), "Saturation", saturation);



        targetMat.SetFloat("speed", speed);

        targetMat.SetFloat("iterations", iterations);
        targetMat.SetFloat("formuparam", formuparam);
        targetMat.SetFloat("volsteps", volsteps);
        targetMat.SetFloat("stepsize", stepsize);
        targetMat.SetFloat("zoom", zoom);
        targetMat.SetFloat("tile", tile);
        targetMat.SetFloat("brightness", brightness);
        targetMat.SetFloat("darkmatter", darkmatter);
        targetMat.SetFloat("distfading", distfading);
        targetMat.SetFloat("saturation", saturation);

        float h = targetMat.GetFloat("_HueShift");
        float s = targetMat.GetFloat("_Saturation");
        float v = targetMat.GetFloat("_Lightness");

        EditorGUILayout.Space();

        h = EditorGUILayout.Slider("Hue", h, 0.0f, 1.0f);
        s = EditorGUILayout.Slider("Sat", s, 0.0f, 5.0f);
        v = EditorGUILayout.Slider("Val", v, 0.0f, 5.0f);

        EditorGUILayout.Space();

        int rq = targetMat.renderQueue;
        rq = EditorGUILayout.IntSlider("RenderQueue", rq, 1000, 5000);
        targetMat.renderQueue = rq;

        targetMat.SetFloat("_HueShift", h);
        targetMat.SetFloat("_Saturation", s);
        targetMat.SetFloat("_Lightness", v);

        //

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(targetMat);
        }

        // render the default inspector
        base.OnInspectorGUI();
    }
}