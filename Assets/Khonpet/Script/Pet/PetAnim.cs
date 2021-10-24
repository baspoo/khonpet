using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetAnim : MonoBehaviour
{
	public enum AnimState
	{
		none = -1,
		Idle, SubIdle_0, SubIdle_1, SubIdle_2, Eat, Sleep, BallKick, Dance, GoodJob, LikeLove, Bad, Need, Petting, Hello
	}
	public Animator Animator;
    public AnimationClip Idle;
    public AnimationClip[] SubIdle;
    public AnimationClip Eat, Sleep, BallKick, Dance, GoodJob, LikeLove, Bad, Need, Petting, Hello;
	public AnimCallback Animcallback => m_animcallback;
	AnimCallback m_animcallback;




    private void Awake()
    {
		Setup();

	}

    //Setup
    bool IsReady;
    public void Setup() 
    {
		var animatorOverrideController = new AnimatorOverrideController(Store.instance.Pet.animatorController);
		animatorOverrideController["Idle"] = Idle;
		animatorOverrideController["Eat"] = Eat;
		animatorOverrideController["Sleep"] = Sleep;
		animatorOverrideController["BallKick"] = BallKick;
		animatorOverrideController["Dance"] = Dance;
		animatorOverrideController["GoodJob"] = GoodJob;
		animatorOverrideController["LikeLove"] = LikeLove;
		animatorOverrideController["Bad"] = Bad;
		animatorOverrideController["Need"] = Need;
		animatorOverrideController["Petting"] = Petting;
		animatorOverrideController["Hello"] = Hello;
		if (SubIdle.Length > 0) animatorOverrideController["SubIdle_0"] = SubIdle[0];
		if (SubIdle.Length > 1) animatorOverrideController["SubIdle_1"] = SubIdle[1];
		if (SubIdle.Length > 2) animatorOverrideController["SubIdle_2"] = SubIdle[2];
		Animator.runtimeAnimatorController = animatorOverrideController;
		m_animcallback = Animator.GetComponent<AnimCallback>();
		IsReady = true;
	}








    //Play
    public void OnAnimForce(AnimState act)
	{
		Animator.Play(act.ToString().ToLower(), -1, 0.0f);
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








	//Idle Handle
	[SerializeField]
	float[] IdelTimes;
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
			int index = Random.Range(0, SubIdle.Length);
			if (index == 0) OnAnimState(1);
			if (index == 1) OnAnimState(2);
			if (index == 2) OnAnimState(3);
		}
	}













	void Update()
	{
		if (!IsReady)
			return;
		IdelAnim();
	}
}
