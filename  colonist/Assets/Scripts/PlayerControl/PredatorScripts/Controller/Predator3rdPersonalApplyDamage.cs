using UnityEngine;
using System.Collections;

public class Predator3rdPersonalApplyDamage : MonoBehaviour {
    
    Predator3rdPersonalUnit predatorPlayerUnit = null;   
    public ParticleSystem electricityHitEffect = null;
	
	public string DieAnimation = "PredatorDie";
	public GameEvent[] DieEvents = new GameEvent[]{};
	
	public bool canDie = true;
    void Awake()
    {
       predatorPlayerUnit = GetComponent<Predator3rdPersonalUnit>();
    }
	// Use this for initialization
	void Start () {
	     
	}
	
	// Update is called once per frame
	void Update () {
        //HealthPrograss.Value = HP / MaxHP;
	}

    public virtual IEnumerator ApplyDamage(DamageParameter param)
    {
        predatorPlayerUnit.HP -= param.damagePoint;
		predatorPlayerUnit.HUDObject.SendMessage("ApplyDamage", param);
		predatorPlayerUnit.Rage = Mathf.Clamp(this.predatorPlayerUnit.Rage + this.predatorPlayerUnit.RageEarnPerBeingHit, 0, this.predatorPlayerUnit.MaxRage);
//        Debug.Log("ApplyDamage at PredatorPlayer, current HP:" + predatorPlayerUnit.HP);
        switch (param.damageForm)
        {
            case DamageForm.ElectricityBoltHit:
                electricityHitEffect.Play();
                break;
        }
		if(predatorPlayerUnit.HP <= 0)
		{
			yield return StartCoroutine("Die", param);
		}
        
    }


    public virtual IEnumerator Die(DamageParameter param)
    {
		if(canDie == false)
			yield break;
//        Destroy(this.transform.root.gameObject);
//        yield return null;
		foreach(GameEvent e in DieEvents)
		{
			LevelManager.OnGameEvent(e, this);
		}
		animation.Play(this.DieAnimation);
		this.GetComponent<CharacterController>().enabled = false;
		foreach(MonoBehaviour mono in this.GetComponents<MonoBehaviour>())
		{
			mono.enabled = false;
		}
		yield break;
    }


}
