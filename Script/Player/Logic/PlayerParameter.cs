using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParameter : MonoBehaviour, ISaveable
{
    public float health;
    public float armor;
    public Player playerController;
    public AnimationOverride animatorController;
    public string GUID => GetComponent<DataGUID>().guid;
    private void OnEnable()
    {
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.ChangePlayerEquiment += OnChangePlayerEquiment;
    }
    private void OnDisable()
    {
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.ChangePlayerEquiment -= OnChangePlayerEquiment;
    }

    private void Awake()
    {
        playerController = gameObject.GetComponent<Player>();
        animatorController = gameObject.GetComponent<AnimationOverride>();
    }
    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }
    private void OnChangePlayerEquiment(ItemDetail itemDetail)
    {
        if (itemDetail == null)
            armor = 0;
        else if (itemDetail.itemType == ItemType.Clothes)
            armor = itemDetail.damage;
    }
    public void CoverState(float recoveryValue)
    {
        health += recoveryValue;
        if (health >= Settings.playerMaxHealth)
            health = Settings.playerMaxHealth;
        EventHandler.CallHealthChangeEvent(health / Settings.playerMaxHealth);
        EventHandler.CallParticleGenerateEvent(transform.position + Vector3.up, ParticaleEffectType.HealthRecover, recoveryValue);
    }
    public void HurtState(float meleeDamage, Vector3 attackDirection)
    {
        float attackDamage = meleeDamage - armor;
        health -= attackDamage == 0 ? 0 : attackDamage;
        EventHandler.CallParticleGenerateEvent(transform.position + Vector3.up, ParticaleEffectType.HealthDamage, attackDamage);
        if (health > 0)
        {
            animatorController.OnSwitchAnimation(PlayerPartType.Hurt);
            playerController.HurtState(attackDirection);
            EventHandler.CallHealthChangeEvent(health / Settings.playerMaxHealth);
        }
        else
        {
            animatorController.OnSwitchAnimation(PlayerPartType.Die);
            playerController.DieState(attackDirection);
            StartCoroutine(PlayerGameOver());
        }
    }
    public IEnumerator PlayerGameOver()
    {
        yield return new WaitForSeconds(1f);
        EventHandler.CallGameOverEvent();
    }
    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.health = this.health;
        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        playerController.ResetPlayerParameter();
        this.health = saveData.health;
        EventHandler.CallSwitchAnimation(PlayerPartType.None);
        EventHandler.CallHealthChangeEvent(health / Settings.playerMaxHealth);
    }

    private void OnStartNewGameEvent(int index)
    {
        EventHandler.CallSwitchAnimation(PlayerPartType.None);
        playerController.ResetPlayerParameter();
        health = Settings.playerMaxHealth;
        EventHandler.CallHealthChangeEvent(health / Settings.playerMaxHealth);
    }
}
