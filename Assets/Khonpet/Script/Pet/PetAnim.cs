using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetAnim : MonoBehaviour
{
	public enum AnimState
	{
		none = -1,
		Idle, SubIdle , Eat, Sleep, BallKick, Dance, GoodJob, LikeLove, Bad, Need, Petting
	}

	
	public Animator Animator;
	[Space]

	public AnimationClip Idle;
    public AnimationClip[] SubIdle;
	[SerializeField]
	float[] IdelTimes = new float[2] { 3, 7 };
	public AnimationClip Eat, Sleep, BallKick, Dance, GoodJob, LikeLove, Bad, Need, Petting;
	public AnimCallback Animcallback => m_animcallback;
	AnimCallback m_animcallback;




    private void Awake()
    {
		Setup();

	}

	//Setup
	public bool IsReady;
	AnimatorOverrideController animatorOverrideController;
	public void Setup() 
    {
		animatorOverrideController = new AnimatorOverrideController(Store.instance.Pet.animatorController);
		animatorOverrideController["Idle"] = FindClip( AnimState.Idle);
		Animator.runtimeAnimatorController = animatorOverrideController;
		m_animcallback = Animator.GetComponent<AnimCallback>();
		IsReady = true;
	}
	AnimationClip FindClip(AnimState state)
	{
        switch (state)
        {
            case AnimState.none: return null;
                break;
            case AnimState.Idle: 
				return Idle;
				break;
            case AnimState.SubIdle:
				return SubIdle[SubIdle.Length.Random()];
				break;
            case AnimState.Eat:
				return Eat;
				break;
            case AnimState.Sleep:
				return Sleep;
				break;
            case AnimState.BallKick:
				return BallKick;
				break;
            case AnimState.Dance:
				return Dance;
				break;
            case AnimState.GoodJob:
				return GoodJob;
				break;
            case AnimState.LikeLove:
				return LikeLove;
				break;
            case AnimState.Bad:
				return Bad;
				break;
            case AnimState.Need:
				return Need;
				break;
            case AnimState.Petting:
				return Petting;
				break;
            default:
                break;
        }
		return null;
    }







	//Play
	public void OnAnimForce(AnimState act)
	{
		StartCoroutine(IEOnAnimForce(act));
	}
	IEnumerator IEOnAnimForce(AnimState act)
	{
		Animator.Play("Idle", -1, 0.0f);
		yield return new WaitForEndOfFrame();
		animatorOverrideController["AnimForce"] = FindClip(act);
		Animator.Play("AnimForce", -1, 0.0f);
	}
	public void OnAnimState(int act)
	{
		StartCoroutine(IEAnimState(act));
	}
	IEnumerator IEAnimState(int act) 
	{
		IdelAnimRuntime = 0.0f;
		Animator.SetInteger("animstate",act);
		yield return new WaitForSeconds(0.1f);
		Animator.SetInteger("animstate", 0);
	}

	public void OnReset()
	{
		Animcallback.OnReset();
	}



	//Idle Handle
	float IdelAnimRuntime = 0.0f;
	float IdelAnimMaxtime = 5.0f;
	void IdelAnim()
	{
		if (Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimState.Idle.ToString()))
			IdelAnimRuntime += Time.deltaTime;
		else
			IdelAnimRuntime = 0.0f;

		if (IdelAnimRuntime >= IdelAnimMaxtime)
		{
			IdelAnimRuntime = 0.0f;
			IdelAnimMaxtime = Random.Range(IdelTimes[0], IdelTimes[1]);
			animatorOverrideController["SubIdle"] = FindClip( AnimState.SubIdle );
			OnAnimState(1);
		}
	}













	void Update()
	{
		if (!IsReady)
			return;
		IdelAnim();
	}
}
