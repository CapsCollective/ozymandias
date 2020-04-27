using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class ScenarioEditor : EditorWindow
{
    Event scenario;
    private VisualElement root;
    private Choice selectedChoice;
    private SerializedObject selectedOutcome;

    private TextField tfScenarioTitle;
    private TextField tfScenarioDescription;
    private ObjectField ofBackground;
    private ListView choiceListView;
    private ListView listOutcomes;
    private ToolbarMenu menuOutcomes;

    [MenuItem("Window/Scenario Editor")]
    static void Init()
    {
        ScenarioEditor editor = (ScenarioEditor)EditorWindow.GetWindow(typeof(ScenarioEditor));
        editor.Show();
    }

    private void OnEnable()
    {
        root = base.rootVisualElement;

        var rootVisualElement = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(@"Assets/Scripts/Events/Editor/ScenarioUI.uxml");
        rootVisualElement.CloneTree(root);

        Button btnSave = root.Query<Button>("BtnSave");
        btnSave.clickable.clicked += SaveScenario;

        Button btnLoad = root.Query<Button>("BtnLoad");
        btnLoad.clickable.clicked += LoadScenario;

        Button btnNewChoice = root.Query<Button>("btnNewChoice");
        btnNewChoice.clickable.clicked += NewChoice;

        //Button btnNewOutcome = root.Query<Button>("btnNewOutcome");
        //btnNewOutcome.clickable.clicked += NewOutcome;

        tfScenarioTitle = root.Query<TextField>("tfScenarioName");
        tfScenarioDescription = root.Query<TextField>("tfScenarioDescription");
        ofBackground = root.Query<ObjectField>("ofBackground");
        listOutcomes = root.Query<ListView>("listOutcomes");
        ofBackground.objectType = typeof(Sprite);
        //VisualElement choiceUI = root.Query("ChoiceTemplate");

        menuOutcomes = root.Query<ToolbarMenu>("menuOutcomes");
        var outcomes = AssetDatabase.FindAssets($"t:{typeof(Outcome)}");
        for (int i = 0; i < outcomes.Length; i++)
        {
            var outcomePath = AssetDatabase.GUIDToAssetPath(outcomes[i]);
            var outcome = AssetDatabase.LoadAssetAtPath<Outcome>(outcomePath);
            Action<DropdownMenuAction> dropdownAction = (a) =>
            {
                if (selectedChoice != null)
                {
                    Type t = outcome.GetType();
                    selectedChoice.PossibleOutcomes.Add(outcome);
                    RefreshOutcomesList();
                }
            };
            menuOutcomes.menu.InsertAction(i, outcome.name, dropdownAction, DropdownMenuAction.Status.Normal);
        }

        IMGUIContainer imgui = root.Query<IMGUIContainer>("IMGUI");
        imgui.onGUIHandler += OnPropertyGUI;

        RefreshScenario();
        
    }

    private void NewOutcome()
    {
        Outcome newOutcome = new Outcome()
        {
            OutcomeName = "New Outcome"
        };
        newOutcome.OutcomeName = "New Outcome";
        AssetDatabase.AddObjectToAsset(newOutcome, selectedChoice);
        selectedChoice.PossibleOutcomes.Add(newOutcome);
        RefreshOutcomesList();
    }

    private void RefreshOutcomesList()
    {
        Func<VisualElement> makeItem = () =>
        {
            var visualElement = new VisualElement();
            visualElement.style.flexDirection = FlexDirection.Row;
            visualElement.style.flexGrow = 1f;
            visualElement.style.flexShrink = 0f;
            visualElement.style.flexBasis = 0f;

            var txt = new Label();
            txt.style.flexGrow = 1f;
            txt.style.flexShrink = 0f;
            txt.style.flexBasis = 0f;
            txt.style.alignSelf = Align.Center;
            visualElement.Add(txt);

            var deleteBtn = new Button();
            deleteBtn.style.flexGrow = 0.01f;
            deleteBtn.style.flexShrink = 0f;
            deleteBtn.style.flexBasis = 0f;
            deleteBtn.style.alignContent = Align.Center;
            deleteBtn.text = "X";
            visualElement.Add(deleteBtn);

            return visualElement;
        };

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            (e.ElementAt(0) as Label).text = selectedChoice.PossibleOutcomes[i].name;
            (e.ElementAt(1) as Button).clicked += () => 
            {
                AssetDatabase.RemoveObjectFromAsset(selectedChoice.PossibleOutcomes[i]);
                selectedChoice.PossibleOutcomes.RemoveAt(i);
                RefreshOutcomesList();
            };
        };

        listOutcomes.itemsSource = selectedChoice.PossibleOutcomes;
        listOutcomes.itemHeight = 20;
        listOutcomes.makeItem = makeItem;
        listOutcomes.bindItem = bindItem;
        listOutcomes.onItemChosen += (o) =>
        {
            selectedOutcome = new SerializedObject((Outcome)o);
        };
    }

    void OnPropertyGUI()
    {
        if (selectedOutcome == null)
            return;

        SerializedProperty property = selectedOutcome.GetIterator();
        if (property.NextVisible(true))
        {
            do
            {
                if (property.name != "m_Script")
                {
                    EditorGUIUtility.labelWidth = 100;
                    EditorGUILayout.PropertyField(property.Copy());
                }
            } while (property.NextVisible(false));
        }
        selectedOutcome.ApplyModifiedProperties();
    }

    private void NewChoice()
    {
        SaveScenario();
        Choice newChoice = new Choice()
        {
            ChoiceTitle = "New Choice"
        };
        newChoice.name = "New Choice";
        AssetDatabase.AddObjectToAsset(newChoice, scenario);
        scenario.Choices.Add(newChoice);
        RefreshScenarioList();
    }

    private void RefreshScenarioList()
    {
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            (e as Label).text = scenario.Choices[i].ChoiceTitle;
            (e as Label).style.unityTextAlign = TextAnchor.MiddleLeft;
        };

        choiceListView = root.Query<ListView>("listChoices");
        choiceListView.itemsSource = scenario.Choices;
        choiceListView.itemHeight = 20;
        choiceListView.makeItem = makeItem;
        choiceListView.bindItem = bindItem;
        choiceListView.onItemChosen += (e) =>
        {
            LoadChoice((Choice)e);
            selectedChoice = (Choice)e;
            RefreshOutcomesList();
        };
    }

    private void LoadChoice(Choice c)
    {
        Debug.Log("Testing");
        TextField tfChoiceName = root.Query<TextField>("tfChoiceName");
        tfChoiceName.SetValueWithoutNotify(c.ChoiceTitle);
        tfChoiceName.RegisterValueChangedCallback((s) => {
            scenario.Choices[choiceListView.selectedIndex].ChoiceTitle = s.newValue;
            scenario.Choices[choiceListView.selectedIndex].name = s.newValue;
            choiceListView.Refresh();
        });

        TextField tfChoiceText = root.Query<TextField>("tfChoiceDescription");
        tfChoiceText.SetValueWithoutNotify(c.ChoiceText);
        tfChoiceText.RegisterValueChangedCallback((s) => scenario.Choices[choiceListView.selectedIndex].ChoiceText = s.newValue);
    }

    private void RefreshScenario()
    {
        if (scenario == null)
        {
            Event newScenario = new Event()
            {
                name = "New Event",
                ScenarioTitle = "New Event",
            };

            scenario = newScenario;
        }
        tfScenarioTitle.SetValueWithoutNotify(scenario.ScenarioTitle);
        tfScenarioDescription.SetValueWithoutNotify(scenario.ScenarioText);
        ofBackground.value = scenario.ScenarioBackground;
        RefreshScenarioList();
    }

    private void LoadScenario()
    {
        string file = EditorUtility.OpenFilePanel("Load Scenario", "Assets/", "asset");
        file = file.Replace(Application.dataPath, "Assets");
        scenario = (Event)AssetDatabase.LoadAssetAtPath(file, typeof(Event));
        if (scenario == null)
        {
            Debug.LogError($"Couldn't load asset: {file}");
            return;
        }
        else
        {
            RefreshScenario();
        }
           
        RefreshScenarioList();
    }

    private void SaveScenario()
    {
        if (!AssetDatabase.Contains(scenario))
        {
            string file = EditorUtility.SaveFilePanel("Save Scenario", "Assets/", "New Event", "asset");
            file = file.Replace(Application.dataPath, "Assets");
            AssetDatabase.CreateAsset(scenario, $"{file}");
        }
    }
}
