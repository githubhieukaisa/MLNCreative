using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

// --- 1. JSON WRAPPER CLASSES ---
[System.Serializable]
public class JsonLine
{
    public string speakerName; // Tên file CharacterProfileSO (VD: "Profile_MC")
    public string content;
    public string bgImageName; // Tên file Sprite Background (VD: "BG_RivalCarts")
    public string charImageName; // Tên file Sprite Nhân vật (VD: "Char_MC_Sweating")
    public string animTrigger;
}

[System.Serializable]
public class JsonDialogueStep
{
    public string stepName; // Tên file DialogueStep (VD: "Step_C2_T1_01_Intro")
    public string dataName; // Tên file DialogueData (VD: "Thoai_C2_T1_DanDat")
    public List<JsonLine> lines;
}

[System.Serializable]
public class JsonOption
{
    public string text;
    public int capitalChange;
    public int brandChange;
    public int techChange;
    public string feedback;
    public string nextBranchName; // Tên file LevelData của nhánh rẽ (VD: "LevelData_C2_LoopA")
}

[System.Serializable]
public class JsonQuestionStep
{
    public string stepName; // Tên file QuestionStep (VD: "Step_C2_T1_02_CauHoi")
    public string questionText;
    public JsonOption optionA;
    public JsonOption optionB;
    public JsonOption optionC;
}

[System.Serializable]
public class JsonLevel
{
    public string levelName; // Tên file LevelData (VD: "LevelData_C2_LoopA" hoặc "LevelData_Chuong_2")
    public List<string> stepNames; // Danh sách tên các Step theo thứ tự
}

[System.Serializable]
public class ChapterDataWrapper
{
    public string levelFolder; // Tên thư mục chứa (VD: "Level2")
    public List<JsonDialogueStep> dialogueSteps;
    public List<JsonQuestionStep> questionSteps;
    public List<JsonLevel> levels;
}

// --- 2. EDITOR WINDOW TOOL ---
public class SOGeneratorWindow : EditorWindow
{
    private string _jsonInput = "";
    private string _basePath = "Assets/_OurData/Scenario";

    // Cache dictionary để Linking ở bước 2
    private Dictionary<string, ScenarioStep> _createdSteps = new Dictionary<string, ScenarioStep>();
    private Dictionary<string, LevelData> _createdLevels = new Dictionary<string, LevelData>();

    [MenuItem("Tools/Visual Novel/Scenario Generator")]
    public static void ShowWindow()
    {
        GetWindow<SOGeneratorWindow>("Scenario Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Dán JSON Kịch Bản Vào Đây:", EditorStyles.boldLabel);
        _jsonInput = EditorGUILayout.TextArea(_jsonInput, GUILayout.Height(300));

        GUILayout.Space(10);
        if (GUILayout.Button("TIẾN HÀNH TẠO SCRIPTABLE OBJECTS", GUILayout.Height(50)))
        {
            GenerateAll();
        }
    }

    private void GenerateAll()
    {
        if (string.IsNullOrEmpty(_jsonInput)) return;

        try
        {
            ChapterDataWrapper data = JsonUtility.FromJson<ChapterDataWrapper>(_jsonInput);
            if (data == null || string.IsNullOrEmpty(data.levelFolder))
            {
                EditorUtility.DisplayDialog("Lỗi", "JSON không đúng định dạng!", "OK");
                return;
            }

            _createdSteps.Clear();
            _createdLevels.Clear();

            string targetFolder = $"{_basePath}/{data.levelFolder}";
            EnsureFoldersExist(targetFolder);

            // BƯỚC 1: TẠO DIALOGUE DATA & DIALOGUE STEPS
            if (data.dialogueSteps != null)
            {
                foreach (var dStep in data.dialogueSteps)
                {
                    // Tạo DialogueData
                    DialogueData dData = ScriptableObject.CreateInstance<DialogueData>();
                    dData.lines = new List<DialogueLine>();
                    foreach (var line in dStep.lines)
                    {
                        DialogueLine newLine = new DialogueLine
                        {
                            speaker = FindAssetByName<CharacterProfileSO>(line.speakerName, "ScriptableObject"),
                            content = line.content,
                            lineVisualState = new VisualState
                            {
                                BackgroundSprite = FindAssetByName<Sprite>(line.bgImageName, "Sprite"),
                                CharacterSprite = FindAssetByName<Sprite>(line.charImageName, "Sprite"),
                                CharacterAnimTrigger = line.animTrigger
                            }
                        };
                        dData.lines.Add(newLine);
                    }
                    SaveAsset(dData, $"{targetFolder}/Data/{dStep.dataName}.asset");

                    // Tạo DialogueStep
                    DialogueStep stepSO = ScriptableObject.CreateInstance<DialogueStep>();
                    stepSO.dialogueData = dData;
                    SaveAsset(stepSO, $"{targetFolder}/Step/Dialogue/{dStep.stepName}.asset");
                    _createdSteps.Add(dStep.stepName, stepSO);
                }
            }

            // BƯỚC 2: TẠO QUESTION STEPS (Chưa link rẽ nhánh)
            if (data.questionSteps != null)
            {
                foreach (var qStep in data.questionSteps)
                {
                    QuestionStep qSO = ScriptableObject.CreateInstance<QuestionStep>();
                    qSO.questionText = qStep.questionText;
                    qSO.optionA = ParseOption(qStep.optionA);
                    qSO.optionB = ParseOption(qStep.optionB);
                    qSO.optionC = ParseOption(qStep.optionC);

                    SaveAsset(qSO, $"{targetFolder}/Question/{qStep.stepName}.asset");
                    _createdSteps.Add(qStep.stepName, qSO);
                }
            }

            // BƯỚC 3: TẠO LEVEL DATA (Loops & Main Levels)
            if (data.levels != null)
            {
                foreach (var lvl in data.levels)
                {
                    LevelData lvlSO = ScriptableObject.CreateInstance<LevelData>();
                    lvlSO.levelName = lvl.levelName;
                    lvlSO.steps = new List<ScenarioStep>();

                    // Link steps
                    foreach (string sName in lvl.stepNames)
                    {
                        if (_createdSteps.ContainsKey(sName))
                        {
                            lvlSO.steps.Add(_createdSteps[sName]);
                        }
                    }

                    // Nếu tên bắt đầu bằng "LevelData_Chuong", lưu ra ngoài LevelX. Nếu là Loop, lưu vào Step/
                    string path = lvl.levelName.StartsWith("LevelData_Chuong")
                        ? $"{targetFolder}/{lvl.levelName}.asset"
                        : $"{targetFolder}/Level/{lvl.levelName}.asset";

                    SaveAsset(lvlSO, path);
                    _createdLevels.Add(lvl.levelName, lvlSO);
                }
            }

            // BƯỚC 4: MÓC NỐI LẠI QUESTION BRANCHES (Wiring)
            if (data.questionSteps != null)
            {
                foreach (var qStep in data.questionSteps)
                {
                    QuestionStep qSO = _createdSteps[qStep.stepName] as QuestionStep;
                    if (qSO != null)
                    {
                        LinkBranch(qSO.optionA, qStep.optionA);
                        LinkBranch(qSO.optionB, qStep.optionB);
                        LinkBranch(qSO.optionC, qStep.optionC);
                        EditorUtility.SetDirty(qSO); // Báo cho Unity biết SO này đã bị sửa đổi
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Hoàn Thành!", "Đã tạo và móc nối toàn bộ Scriptable Objects thành công!", "Tuyệt Vời");
            _jsonInput = "";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi tạo SO: {e.Message}");
            EditorUtility.DisplayDialog("Lỗi", "Kiểm tra Console để xem chi tiết.", "OK");
        }
    }

    // --- CÁC HÀM TIỆN ÍCH HỖ TRỢ ---
    private Option ParseOption(JsonOption jOpt)
    {
        if (jOpt == null || string.IsNullOrEmpty(jOpt.text)) return new Option();
        return new Option
        {
            text = jOpt.text,
            capitalChange = jOpt.capitalChange,
            brandChange = jOpt.brandChange,
            techChange = jOpt.techChange,
            feedback = jOpt.feedback
        };
    }

    private void LinkBranch(Option opt, JsonOption jOpt)
    {
        if (jOpt != null && !string.IsNullOrEmpty(jOpt.nextBranchName) && _createdLevels.ContainsKey(jOpt.nextBranchName))
        {
            opt.nextBranch = _createdLevels[jOpt.nextBranchName];
        }
    }

    private T FindAssetByName<T>(string name, string typeFilter) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(name)) return null;
        string[] guids = AssetDatabase.FindAssets($"{name} t:{typeFilter}");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
        Debug.LogWarning($"Không tìm thấy asset: {name} (Loại: {typeFilter})");
        return null;
    }

    private void SaveAsset(ScriptableObject so, string path)
    {
        // Ghi đè nếu đã tồn tại
        if (AssetDatabase.LoadAssetAtPath<ScriptableObject>(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }
        AssetDatabase.CreateAsset(so, path);
    }

    private void EnsureFoldersExist(string rootFolder)
    {
        string[] subfolders = { "Data", "Level", "Question", "Step", "Step/Dialogue" };
        if (!AssetDatabase.IsValidFolder(rootFolder))
        {
            string parent = Path.GetDirectoryName(rootFolder).Replace("\\", "/");
            string newFolder = Path.GetFileName(rootFolder);
            AssetDatabase.CreateFolder(parent, newFolder);
        }
        foreach (var sub in subfolders)
        {
            string fullPath = $"{rootFolder}/{sub}";
            if (!AssetDatabase.IsValidFolder(fullPath))
            {
                string parent = Path.GetDirectoryName(fullPath).Replace("\\", "/");
                string newFolder = Path.GetFileName(fullPath);
                AssetDatabase.CreateFolder(parent, newFolder);
            }
        }
    }
}