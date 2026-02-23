using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Linq;

// 1. CẤU TRÚC JSON WRAPPERS
[System.Serializable]
public class AnimJsonWrapper
{
    public List<AnimClipData> animations;
}

[System.Serializable]
public class AnimClipData
{
    public string clipName;
    public int frameRate = 60;
    public List<AnimProperty> properties;
    public AnimSetup animatorSetup;
}

[System.Serializable]
public class AnimProperty
{
    public string bindingType;  // VD: "UnityEngine.RectTransform", "UnityEngine.Transform"
    public string propertyName; // VD: "m_AnchoredPosition.y", "localEulerAnglesRaw.z", "m_LocalScale.y"
    public List<AnimKeyframe> keyframes;
}

[System.Serializable]
public class AnimKeyframe
{
    public int frame;
    public float value;
}

[System.Serializable]
public class AnimSetup
{
    public string triggerName;
    public string returnStateName; // VD: "Char_Idle"
}

// 2. CỬA SỔ EDITOR
public class AnimationGeneratorWindow : EditorWindow
{
    private string _jsonInput = "";
    private string _saveFolder = "Assets/_OurData/Animation";
    private string _controllerPath = "Assets/_OurData/Animation/Char_AnimController.controller";

    [MenuItem("Tools/Visual Novel/Animation Generator")]
    public static void ShowWindow()
    {
        GetWindow<AnimationGeneratorWindow>("Anim Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Paste JSON Data Here:", EditorStyles.boldLabel);
        _jsonInput = EditorGUILayout.TextArea(_jsonInput, GUILayout.Height(300));

        GUILayout.Space(10);
        _saveFolder = EditorGUILayout.TextField("Save Folder", _saveFolder);
        _controllerPath = EditorGUILayout.TextField("Controller Path", _controllerPath);

        GUILayout.Space(10);
        if (GUILayout.Button("Generate Animations & Setup Controller", GUILayout.Height(40)))
        {
            GenerateFromJSON();
        }
    }

    private void GenerateFromJSON()
    {
        if (string.IsNullOrEmpty(_jsonInput))
        {
            EditorUtility.DisplayDialog("Lỗi", "Vui lòng nhập JSON!", "OK");
            return;
        }

        try
        {
            AnimJsonWrapper wrapper = JsonUtility.FromJson<AnimJsonWrapper>(_jsonInput);
            if (wrapper == null || wrapper.animations == null) return;

            // Load Animator Controller
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(_controllerPath);
            if (controller == null)
            {
                Debug.LogError($"[Tech Lead] Không tìm thấy AnimatorController tại: {_controllerPath}. Hãy kiểm tra lại đường dẫn.");
                return;
            }

            foreach (var animData in wrapper.animations)
            {
                CreateAnimationAndSetup(animData, controller);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Thành công", $"Đã tạo và setup {wrapper.animations.Count} animations!", "Tuyệt");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Tech Lead] Lỗi JSON hoặc Logic: {e.Message}");
        }
    }

    private void CreateAnimationAndSetup(AnimClipData data, AnimatorController controller)
    {
        // 1. TẠO ANIMATION CLIP
        AnimationClip clip = new AnimationClip { frameRate = data.frameRate };

        foreach (var prop in data.properties)
        {
            AnimationCurve curve = new AnimationCurve();
            foreach (var kf in prop.keyframes)
            {
                float time = (float)kf.frame / data.frameRate;
                curve.AddKey(new Keyframe(time, kf.value));
            }

            // Ép đồ thị thành dạng Flat (Smooth Auto) để chuyển động mượt mà
            for (int i = 0; i < curve.keys.Length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Auto);
                AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Auto);
            }

            System.Type componentType = System.Type.GetType(prop.bindingType + ", UnityEngine.CoreModule");
            if (componentType == null) componentType = typeof(Transform); // Fallback

            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve("", componentType, prop.propertyName), curve);
        }

        string clipPath = $"{_saveFolder}/{data.clipName}.anim";
        AssetDatabase.CreateAsset(clip, clipPath);

        // 2. SETUP ANIMATOR CONTROLLER
        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

        // Thêm Parameter Trigger (nếu chưa có)
        bool paramExists = controller.parameters.Any(p => p.name == data.animatorSetup.triggerName);
        if (!paramExists)
        {
            controller.AddParameter(data.animatorSetup.triggerName, AnimatorControllerParameterType.Trigger);
        }

        // Tạo State mới chứa Clip
        AnimatorState newState = null;
        foreach (var childState in rootStateMachine.states)
        {
            if (childState.state.name == data.clipName) newState = childState.state;
        }
        if (newState == null)
        {
            newState = rootStateMachine.AddState(data.clipName);
            newState.motion = clip;
        }
        else
        {
            newState.motion = clip; // Cập nhật clip nếu state đã tồn tại
        }

        // Tạo Transition: Any State -> New State
        var anyTransitions = rootStateMachine.anyStateTransitions;
        bool hasAnyTrans = anyTransitions.Any(t => t.destinationState == newState);
        if (!hasAnyTrans)
        {
            AnimatorStateTransition anyTrans = rootStateMachine.AddAnyStateTransition(newState);
            anyTrans.AddCondition(AnimatorConditionMode.If, 0, data.animatorSetup.triggerName);
            anyTrans.duration = 0f; // Duration = 0 như yêu cầu
            anyTrans.canTransitionToSelf = false;
        }

        // Tạo Transition: New State -> Return State (Char_Idle)
        AnimatorState returnState = rootStateMachine.states.FirstOrDefault(s => s.state.name == data.animatorSetup.returnStateName).state;
        if (returnState != null)
        {
            bool hasExitTrans = newState.transitions.Any(t => t.destinationState == returnState);
            if (!hasExitTrans)
            {
                AnimatorStateTransition exitTrans = newState.AddTransition(returnState);
                exitTrans.hasExitTime = true;
                exitTrans.exitTime = 1f; // Chờ hết clip
                exitTrans.duration = 0.1f; // Blend nhẹ khi về Idle
            }
        }
        else
        {
            Debug.LogWarning($"[Tech Lead] Không tìm thấy state '{data.animatorSetup.returnStateName}' để nối dây từ {data.clipName}. Bạn hãy tự nối tay phần này nhé.");
        }
    }
}