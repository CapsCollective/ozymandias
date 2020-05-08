using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


public class ScenarioEditor : EditorWindow
{

    const string EDIT_DEFAULT_BTN_TEXT = "Edit Default Outcome: ";

    Event scenario;
    private VisualElement root;
    private Choice selectedChoice;
    private SerializedObject selectedOutcome;

    private ListView listViewLibrary;
    private TextField tfScenarioTitle;
    private TextField tfScenarioDescription;
    private ObjectField ofBackground;
    private ListView choiceListView;
    private ListView listOutcomes;
    private ToolbarMenu menuEventOutcomes;
    private ToolbarMenu menuChoiceOutcomes;
    Button btnEditDefaultOutcome;
    private List<Event> eventsList = new List<Event>();


    private Color[] colors =
    {
        new Color(0.74f, 0.74f, 0.74f),
        new Color(0.79f, 0.79f, 0.79f),
    };


    [MenuItem("Window/Event Editor")]
    static void Init()
    {
        ScenarioEditor editor = (ScenarioEditor)EditorWindow.GetWindow(typeof(ScenarioEditor));
        editor.name = "Event Editor";
        editor.Show();
    }

    private void OnEnable()
    {
        root = base.rootVisualElement;

        var rootVisualElement = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(@"Assets/Scripts/Events/Editor/ScenarioUI.uxml");
        rootVisualElement.CloneTree(root);

        Button btnNewChoice = root.Query<Button>("btnNewChoice");
        btnNewChoice.clickable.clicked += NewChoice;

        Button btnNewEvent = root.Query<Button>("btnNewEvent");
        btnNewEvent.clickable.clicked += SaveScenario;

        //Button btnNewOutcome = root.Query<Button>("btnNewOutcome");
        //btnNewOutcome.clickable.clicked += NewOutcome;

        listViewLibrary = root.Query<ListView>("listViewLibrary");
        tfScenarioTitle = root.Query<TextField>("tfScenarioName");
        tfScenarioDescription = root.Query<TextField>("tfScenarioDescription");
        ofBackground = root.Query<ObjectField>("ofBackground");
        listOutcomes = root.Query<ListView>("listOutcomes");
        ofBackground.objectType = typeof(Sprite);
        //VisualElement choiceUI = root.Query("ChoiceTemplate");

        btnEditDefaultOutcome = root.Query<Button>("btnEditDefaultOutcome");
        btnEditDefaultOutcome.clickable.clicked += () =>
        {
            if (scenario.defaultOutcome != null)
                selectedOutcome = new SerializedObject(scenario.defaultOutcome);
        };

        var outcomes = AssetDatabase.FindAssets($"t:{typeof(Outcome)}");
        menuEventOutcomes = root.Query<ToolbarMenu>("menuEventOutcomes");
        for (int i = 0; i < outcomes.Length; i++)
        {
            var outcomePath = AssetDatabase.GUIDToAssetPath(outcomes[i]);
            var outcome = AssetDatabase.LoadAssetAtPath<Outcome>(outcomePath);
            Action<DropdownMenuAction> dropdownAction = (a) =>
            {
                scenario.defaultOutcome = Instantiate(outcome);
                btnEditDefaultOutcome.text = EDIT_DEFAULT_BTN_TEXT + scenario.defaultOutcome.name;
            };
            menuEventOutcomes.menu.InsertAction(i, outcome.name, dropdownAction, DropdownMenuAction.Status.Normal);
        }

        menuChoiceOutcomes = root.Query<ToolbarMenu>("menuChoiceOutcomes");
        for (int i = 0; i < outcomes.Length; i++)
        {
            var outcomePath = AssetDatabase.GUIDToAssetPath(outcomes[i]);
            var outcome = AssetDatabase.LoadAssetAtPath<Outcome>(outcomePath);
            Action<DropdownMenuAction> dropdownAction = (a) =>
            {
                if (selectedChoice != null)
                {
                    Outcome newOutcome = Instantiate(outcome);
                    selectedChoice.PossibleOutcomes.Add(newOutcome);
                    AssetDatabase.AddObjectToAsset(newOutcome, selectedChoice);
                    RefreshOutcomesList();
                }
            };
            menuChoiceOutcomes.menu.InsertAction(i, outcome.name, dropdownAction, DropdownMenuAction.Status.Normal);
        }

        IMGUIContainer imgui = root.Query<IMGUIContainer>("IMGUI");
        imgui.onGUIHandler += OnPropertyGUI;
        PopulateLibrary();

        RefreshScenario();
        
    }

    private void PopulateLibrary()
    {
        var events = AssetDatabase.FindAssets($"t:{typeof(Event)}");
        foreach (var s in events)
        {
            var eventPath = AssetDatabase.GUIDToAssetPath(s);
            var eevent = AssetDatabase.LoadAssetAtPath<Event>(eventPath);
            eventsList.Add(eevent);
        }
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            (e as Label).style.backgroundColor = colors[i % 2];
            (e as Label).text = eventsList[i].ScenarioTitle;
            (e as Label).style.unityTextAlign = TextAnchor.MiddleLeft;
        };
        eventsList.Sort((x, y) => string.Compare(x.ScenarioTitle, y.ScenarioTitle));
        listViewLibrary.itemsSource = eventsList;
        listViewLibrary.makeItem = makeItem;
        listViewLibrary.bindItem = bindItem;
        listViewLibrary.onItemChosen += (o) =>
        {
            scenario = o as Event;
            RefreshScenario();
        };
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
            e.style.backgroundColor = colors[i % 2];
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
            e.style.backgroundColor = colors[i % 2];
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
        if (scenario != null)
        {
            var serializedObject = new SerializedObject(scenario);
            tfScenarioTitle.SetValueWithoutNotify(scenario.ScenarioTitle);
            tfScenarioTitle.Bind(serializedObject);
            tfScenarioTitle.RegisterValueChangedCallback((s) => listViewLibrary.Refresh());
            tfScenarioDescription.SetValueWithoutNotify(scenario.ScenarioText);
            tfScenarioDescription.Bind(serializedObject);
            tfScenarioDescription.RegisterValueChangedCallback((s) => listViewLibrary.Refresh());
            ofBackground.value = scenario.ScenarioBackground;

            if (scenario.defaultOutcome != null)
                btnEditDefaultOutcome.text = EDIT_DEFAULT_BTN_TEXT + scenario.defaultOutcome.name;
            else
                btnEditDefaultOutcome.text = EDIT_DEFAULT_BTN_TEXT + "None";

            RefreshScenarioList();
        }
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
        string file = EditorUtility.SaveFilePanel("Save Scenario", "Assets/", "New Event", "asset");
        file = file.Replace(Application.dataPath, "Assets");
        Event newEvent = new Event { ScenarioTitle = "New Event", name = "New Event" };
        if (!string.IsNullOrEmpty(file))
        {
            AssetDatabase.CreateAsset(newEvent, $"{file}");
            eventsList.Add(newEvent);
            eventsList.Sort((x, y) => string.Compare(x.ScenarioTitle, y.ScenarioTitle));
            listViewLibrary.Refresh();
        }
    }
}
