using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParameterManager : Singleton<ParameterManager>, ISaveable
{
    [Header("敌人信息")]
    public ParameterDetailData_SO parmaterDetailData_SO;
    public GameObject enemyBase;
    public GameObject animalBase;
    private Transform characterParent;
    private Instanceportal[] sceneInstanceportals;
    private Dictionary<string, List<SceneParameter>> sceneParameterDict = new Dictionary<string, List<SceneParameter>>();

    public string GUID => GetComponent<DataGUID>().guid;

    private void OnEnable()
    {
        EventHandler.GameDateEvent += OnGameDateEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.GenerateParameterEvent += OnGenerateParameterEvent;
        EventHandler.AfterSceneDataLoadEvent += OnAfterSceneDataLoadEvent;
        EventHandler.LoadSceneDataEvent += OnLoadSceneDataEvent;
        EventHandler.BeforeSceneLoadEvent += OnBeforeSceneLoadEvent;
    }
    private void OnDisable()
    {
        EventHandler.GameDateEvent -= OnGameDateEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.GenerateParameterEvent -= OnGenerateParameterEvent;
        EventHandler.AfterSceneDataLoadEvent -= OnAfterSceneDataLoadEvent;
        EventHandler.LoadSceneDataEvent -= OnLoadSceneDataEvent;
        EventHandler.BeforeSceneLoadEvent -= OnBeforeSceneLoadEvent;
    }

    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }

    private void OnGameDateEvent(int gameHour, int gameDay, LightShift lightShift)
    {
        if (lightShift != LightShift.Night || sceneInstanceportals == null)
            return;

        foreach (Instanceportal instanceportal in sceneInstanceportals)
        {
            instanceportal.CheckGameDate(gameHour, gameDay);
        }
    }

    private void OnLoadSceneDataEvent()
    {
        LoadSceneParameter();
    }
    private void OnBeforeSceneLoadEvent()
    {
        SaveSceneParameter();
    }
    private void OnAfterSceneDataLoadEvent()
    {
        sceneInstanceportals = FindObjectsOfType<Instanceportal>();
    }
    private void OnAfterSceneLoadEvent()
    {
        characterParent = GameObject.FindWithTag("CharacterParent").transform;

        string sceneName = TransitionManager.Instance.GetCurrentSceneName();
        if (!sceneParameterDict.ContainsKey(sceneName))
            return;
        for (int i = 0; i < characterParent.childCount; i++)
        {
            Destroy(characterParent.GetChild(i).gameObject);
        }

    }
    public Dictionary<StateType, IState> GetIStateDict(StateDataList stateDataList, EnemyBaseController controller)
    {
        Dictionary<StateType, IState> stateDict = new Dictionary<StateType, IState>();
        stateDict.Add(StateType.Appear, GetIState(stateDataList.appear, controller));
        stateDict.Add(StateType.Idle, GetIState(stateDataList.idle, controller));
        stateDict.Add(StateType.Move, GetIState(stateDataList.move, controller));
        stateDict.Add(StateType.Hurt, GetIState(stateDataList.hurt, controller));
        stateDict.Add(StateType.Die, GetIState(stateDataList.die, controller));
        stateDict.Add(StateType.Attack, GetIState(stateDataList.attack, controller));
        stateDict.Add(StateType.Skill, GetIState(stateDataList.skill, controller));

        return stateDict;
    }
    private IState GetIState(StateDataType stateDataType, EnemyBaseController controller)
    {
        return stateDataType switch
        {
            StateDataType.EnemyAppear => new EnemyAppearState(controller),
            StateDataType.EnemyIdle => new EnemyIdelState(controller),
            StateDataType.EnemyMove => new EnemyMoveState(controller),
            StateDataType.EnemyAttack => new EnemyMeleeAttackState(controller),
            StateDataType.EnemySkillRemote => new EnemyRemoteAttackState(controller),
            StateDataType.EnemySkillSummon => new EnemySummonState(controller),
            StateDataType.EnemyHurt => new EnemyHurtState(controller),
            StateDataType.EnemyDie => new EnemyDieState(controller),
            StateDataType.None => new EnemyNoneState(controller),
            _ => null
        };
    }


    public Dictionary<StateType, IState> GetIStateDict(StateDataList stateDataList, AnimalBaseController controller)
    {
        Dictionary<StateType, IState> stateDict = new Dictionary<StateType, IState>();
        stateDict.Add(StateType.Appear, GetIState(stateDataList.appear, controller));
        stateDict.Add(StateType.Idle, GetIState(stateDataList.idle, controller));
        stateDict.Add(StateType.Move, GetIState(stateDataList.move, controller));
        stateDict.Add(StateType.Hurt, GetIState(stateDataList.die, controller));
        stateDict.Add(StateType.Die, GetIState(stateDataList.die, controller));
        stateDict.Add(StateType.Attack, GetIState(stateDataList.attack, controller));
        stateDict.Add(StateType.Skill, GetIState(stateDataList.skill, controller));
        return stateDict;
    }

    private IState GetIState(StateDataType stateDataType, AnimalBaseController controller)
    {
        return stateDataType switch
        {
            StateDataType.AnimalAppear => new AnimalAppearState(controller),
            StateDataType.AnimalIdle => new AnimalIdleState(controller),
            StateDataType.AnimalMove => new AnimalMoveState(controller),
            StateDataType.AnimalAttack => new AnimalMeleeState(controller),
            StateDataType.AnimalSkill => new AnimalSkillState(controller),
            StateDataType.AnimalHurt => new AnimalHurtState(controller),
            StateDataType.AnimalDie => new AnimalDieState(controller),
            StateDataType.None => new AnimalNoneState(controller),
            _ => null
        };
    }

    public Parameter GetParameterDetail(string parameterID)
    {
        return parmaterDetailData_SO.parameterList.Find(i => i.ID == parameterID);
    }
    public void OnGenerateParameterEvent(string parameterID, Vector3 position)
    {
        Parameter parameter = GetParameterDetail(parameterID);
        if (parameter != null)
        {
            if (parameter.parameterType == ParameterType.Enemy)
            {
                GameObject enemy = Instantiate(enemyBase, characterParent);
                enemy.transform.position = position;
                EnemyBaseController controller = enemy.GetComponent<EnemyBaseController>();
                controller.enemyParameter = parameter;
            }
            else if (parameter.parameterType == ParameterType.Animal)
            {
                GameObject animal = Instantiate(animalBase, characterParent);
                animal.transform.position = position;
                AnimalBaseController controller = animal.GetComponent<AnimalBaseController>();
                controller.animalParameter = parameter;
            }
        }
    }
    // private void GenerateParameterEvent(string parameterID, Vector3 position, float health)
    // {
    //     Parameter parameter = GetParameterDetail(parameterID);
    //     if (parameter != null)
    //     {
    //         if (parameter.parameterType == ParameterType.Enemy)
    //         {
    //             GameObject enemy = Instantiate(enemyBase, characterParent);
    //             enemy.transform.position = position;
    //             EnemyBaseController controller = enemy.GetComponent<EnemyBaseController>();
    //             controller.enemyParameter = parameter;
    //             controller.health = health;
    //         }
    //         else if (parameter.parameterType == ParameterType.Animal)
    //         {
    //             GameObject animal = Instantiate(enemyBase, characterParent);
    //             animal.transform.position = position;
    //             AnimalBaseController controller = animal.GetComponent<AnimalBaseController>();
    //             controller.animalParameter = parameter;
    //             controller.health = health;
    //         }
    //     }
    // }
    public void SaveSceneParameter()
    {
        List<SceneParameter> sceneParameterList = new List<SceneParameter>();
        for (int i = 0; i < characterParent.childCount; i++)
        {
            ICharacter parameter;
            if (characterParent.GetChild(i).TryGetComponent<ICharacter>(out parameter))
            {
                SceneParameter sceneParameter = new SceneParameter();
                sceneParameter.parmeterID = parameter.GetParameterID();
                sceneParameter.position = parameter.GetParameterPosition();
                sceneParameter.health = parameter.GetParameterHealth();
                sceneParameterList.Add(sceneParameter);
            }
        }
        string sceneName = TransitionManager.Instance.GetCurrentSceneName();
        if (!sceneParameterDict.ContainsKey(sceneName))
            sceneParameterDict.Add(sceneName, sceneParameterList);
        else
            sceneParameterDict[sceneName] = sceneParameterList;

        Debug.Log("------场景:" + sceneName + "   （角色）保存: " + characterParent.childCount + " 个组件------");
    }
    public void LoadSceneParameter()
    {
        string sceneName = TransitionManager.Instance.GetCurrentSceneName();
        if (!sceneParameterDict.ContainsKey(sceneName))
            return;

        List<SceneParameter> sceneParameterList = sceneParameterDict[sceneName];

        for (int i = 0; i < sceneParameterList.Count; i++)
        {
            SceneParameter sceneParameter = sceneParameterList[i];
            Parameter parameter = GetParameterDetail(sceneParameter.parmeterID);
            if (parameter != null)
            {
                OnGenerateParameterEvent(sceneParameter.parmeterID, sceneParameter.position.ToVector3());
            }
        }
        Debug.Log("------场景:" + sceneName + "   （角色）保存: " + characterParent.childCount + " 个组件------");

    }

    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.sceneParameterDict = this.sceneParameterDict;

        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        this.sceneParameterDict = saveData.sceneParameterDict;
    }
    private void OnStartNewGameEvent(int index)
    {
        sceneParameterDict.Clear();
    }

}
