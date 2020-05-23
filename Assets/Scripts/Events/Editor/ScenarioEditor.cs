﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.IMGUI.Controls;
using System.Linq;
using UnityEngine.Analytics;

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
    private ListView listChoiceOutcomes;
    private ListView listEventOutcomes;
    private ToolbarMenu menuEventOutcomes;
    private ToolbarMenu menuChoiceOutcomes;
    private ToolbarSearchField librarySearchField;
    private List<Event> eventsList = new List<Event>();
    private EnumField enumEventType;

    // Search Stuff
    private IEnumerable<Event> searchList;
    private string searchString;


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

        eventsList.Clear();

        var rootVisualElement = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(@"Assets/Scripts/Events/Editor/ScenarioUI.uxml");
        rootVisualElement.CloneTree(root);

        Button btnNewChoice = root.Query<Button>("btnNewChoice");
        btnNewChoice.clickable.clicked += NewChoice;

        Button btnNewEvent = root.Query<Button>("btnNewEvent");
        btnNewEvent.clickable.clicked += SaveScenario;

        Button btnSelectAsset = root.Query<Button>("btnSelectAsset");
        btnSelectAsset.clickable.clicked += () => Selection.activeObject = scenario;

        enumEventType = root.Query<EnumField>("enumEventType");
        enumEventType.Init(Event.EventType.Adventurers);
        enumEventType.RegisterValueChangedCallback((e) =>
        {
            if(scenario != null)
                scenario.type = (Event.EventType)e.newValue;
        });

        librarySearchField = root.Query<ToolbarSearchField>("searchLibrary");
        librarySearchField.RegisterCallback<KeyDownEvent>(e =>
        {
            if (e.keyCode == KeyCode.Return)
                OnSearch(librarySearchField.value);
        });

        listViewLibrary = root.Query<ListView>("listViewLibrary");
        tfScenarioTitle = root.Query<TextField>("tfScenarioName");
        tfScenarioDescription = root.Query<TextField>("tfScenarioDescription");
        ofBackground = root.Query<ObjectField>("ofBackground");
        listChoiceOutcomes = root.Query<ListView>("listChoiceOutcomes");
        ofBackground.objectType = typeof(Sprite);
        //VisualElement choiceUI = root.Query("ChoiceTemplate");
        listEventOutcomes = root.Query<ListView>("listEventOutcomes");


        string[] folders = new string[] { "Assets/Events/Outcomes" };
        var outcomes = AssetDatabase.FindAssets($"t:{typeof(Outcome)}", folders);
        menuEventOutcomes = root.Query<ToolbarMenu>("menuEventOutcomes");
        for (int i = 0; i < outcomes.Length; i++)
        {
            var outcomePath = AssetDatabase.GUIDToAssetPath(outcomes[i]);
            var outcome = AssetDatabase.LoadAssetAtPath<Outcome>(outcomePath);
            Action<DropdownMenuAction> dropdownAction = (a) =>
            {
                Outcome newOutcome = Instantiate(outcome);
                scenario.outcomes.Add(newOutcome);
                AssetDatabase.AddObjectToAsset(newOutcome, scenario);
                listEventOutcomes.itemsSource = scenario.outcomes;
                AssetDatabase.SaveAssets();
                listEventOutcomes.Refresh();
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
                    selectedChoice.outcomes.Add(newOutcome);
                    AssetDatabase.AddObjectToAsset(newOutcome, selectedChoice);
                    AssetDatabase.SaveAssets();
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

    private void OnSearch(string s)
    {
        searchList = eventsList.Where((x) => x.headline.ToLower().Contains(s.ToLower()));
        Debug.Log(searchList.ToList().Count());
        listViewLibrary.itemsSource = searchList.ToList();

        listViewLibrary.Refresh();
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
            (e as Label).text = searchList.ToList()[i].headline;
            (e as Label).style.unityTextAlign = TextAnchor.MiddleLeft;
        };
        eventsList.Sort((x, y) => string.Compare(x.headline, y.headline));
        searchList = eventsList;
        listViewLibrary.itemsSource = searchList.ToList();
        listViewLibrary.makeItem = makeItem;
        listViewLibrary.bindItem = bindItem;
        listViewLibrary.onItemChosen += (o) =>
        {
            AssetDatabase.SaveAssets();
            scenario = o as Event;
            RefreshScenario();
        };
    }

    private void SetupEventOutcomes()
    {
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            e.style.backgroundColor = colors[i % 2];
            (e as Label).text = scenario.outcomes[i].name;
            (e.ElementAt(0) as Button).clicked += () =>
            {
                AssetDatabase.RemoveObjectFromAsset(scenario.outcomes[i]);
                scenario.outcomes.RemoveAt(i);
                listEventOutcomes.Refresh();
                AssetDatabase.SaveAssets();
            };
        };

        listEventOutcomes.itemsSource = scenario.outcomes;
        listEventOutcomes.itemHeight = 20;
        listEventOutcomes.makeItem = MakeListItem;
        listEventOutcomes.bindItem = bindItem;
        listEventOutcomes.onItemChosen += (o) =>
        {
            selectedOutcome = new SerializedObject((Outcome)o);
            AssetDatabase.SaveAssets();
        };
    }

    private void NewOutcome()
    {
        Outcome newOutcome = new Outcome()
        {
            name = "New Outcome"
        };
        newOutcome.name = "New Outcome";
        AssetDatabase.AddObjectToAsset(newOutcome, selectedChoice);
        selectedChoice.outcomes.Add(newOutcome);
        RefreshOutcomesList();
    }

    private void RefreshOutcomesList()
    {
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            e.style.backgroundColor = colors[i % 2];
            (e as Label).text = selectedChoice.outcomes[i].name;
            (e.ElementAt(0) as Button).clicked += () => 
            {
                AssetDatabase.RemoveObjectFromAsset(selectedChoice.outcomes[i]);
                selectedChoice.outcomes.RemoveAt(i);
                AssetDatabase.SaveAssets();
                RefreshOutcomesList();
            };
        };

        listChoiceOutcomes.itemsSource = selectedChoice.outcomes;
        listChoiceOutcomes.itemHeight = 20;
        listChoiceOutcomes.makeItem = MakeListItem;
        listChoiceOutcomes.bindItem = bindItem;
        listChoiceOutcomes.onItemChosen += (o) =>
        {
            AssetDatabase.SaveAssets();
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
            description = "New Choice"
        };
        newChoice.name = "New Choice";
        AssetDatabase.AddObjectToAsset(newChoice, scenario);
        scenario.choices.Add(newChoice);
        RefreshScenarioList();
        AssetDatabase.SaveAssets();
    }

    private void RefreshScenarioList()
    {
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            e.style.backgroundColor = colors[i % 2];
            (e as Label).text = scenario.choices[i].name;
            (e as Label).style.unityTextAlign = TextAnchor.MiddleLeft;
            (e.ElementAt(0) as Button).clicked += () =>
            {
                AssetDatabase.RemoveObjectFromAsset(scenario.choices[i]);
                scenario.choices.RemoveAt(i);
                choiceListView.itemsSource = scenario.choices;
                choiceListView.Refresh();
                AssetDatabase.SaveAssets();
            };
        };

        choiceListView = root.Query<ListView>("listChoices");
        choiceListView.itemsSource = scenario.choices;
        choiceListView.itemHeight = 20;
        choiceListView.makeItem = MakeListItem;
        choiceListView.bindItem = bindItem;
        choiceListView.onItemChosen += (e) =>
        {
            LoadChoice((Choice)e);
            selectedChoice = (Choice)e;
            RefreshOutcomesList();
            AssetDatabase.SaveAssets();
        };
    }

    private void LoadChoice(Choice c)
    {
        TextField tfChoiceDescription = root.Query<TextField>("tfChoiceDescription");
        tfChoiceDescription.SetValueWithoutNotify(c.description);
        tfChoiceDescription.RegisterValueChangedCallback((s) => {
            scenario.choices[choiceListView.selectedIndex].description = s.newValue;
            scenario.choices[choiceListView.selectedIndex].name = s.newValue;
            choiceListView.Refresh();
        });

        // TextField tfChoiceText = root.Query<TextField>("tfChoiceDescription");
        // tfChoiceText.SetValueWithoutNotify(c.ChoiceText);
        // tfChoiceText.RegisterValueChangedCallback((s) => scenario.choices[choiceListView.selectedIndex].ChoiceText = s.newValue);
    }

    private void RefreshScenario()
    {
        if (scenario != null)
        {
            var serializedObject = new SerializedObject(scenario);
            tfScenarioTitle.SetValueWithoutNotify(scenario.headline);
            tfScenarioTitle.Bind(serializedObject);
            tfScenarioTitle.RegisterCallback<UnityEngine.UIElements.FocusOutEvent>(e => OnSearch(""));
            tfScenarioDescription.SetValueWithoutNotify(scenario.article);
            tfScenarioDescription.Bind(serializedObject);
            ofBackground.value = scenario.image;
            enumEventType.Init(scenario.type);
            SetupEventOutcomes();

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
        string file = EditorUtility.SaveFilePanel("Save Scenario", "Assets/Events", "New Event", "asset");
        file = file.Replace(Application.dataPath, "Assets");
        Event newEvent = new Event { headline = "New Event", name = "New Event" };
        if (!string.IsNullOrEmpty(file))
        {
            AssetDatabase.CreateAsset(newEvent, $"{file}");
            eventsList.Add(newEvent);
            searchList = eventsList;
            eventsList.Sort((x, y) => string.Compare(x.headline, y.headline));
            listViewLibrary.itemsSource = searchList.ToList();
            listViewLibrary.Refresh();
        }
    }

    private VisualElement MakeListItem()
    {
        var txt = new Label();
        txt.style.flexGrow = 1f;
        txt.style.flexShrink = 0f;
        txt.style.flexBasis = 0f;
        txt.style.flexDirection = FlexDirection.RowReverse;
        txt.style.alignSelf = Align.Center;

        var deleteBtn = new Button();
        deleteBtn.style.flexGrow = 0.01f;
        deleteBtn.style.flexShrink = 0f;
        deleteBtn.style.flexBasis = 0f;
        deleteBtn.style.alignContent = Align.Center;
        deleteBtn.text = "X";
        txt.Add(deleteBtn);

        return txt;
    }
}
