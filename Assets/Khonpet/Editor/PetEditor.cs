using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(PetObj))]
[System.Serializable]
public class PetEditor : Editor
{
	public PetObj pet => (PetObj)target;
	int iTap;
	string[] taps = new string[] { "Info", "Animation", "Costume" };

	public override void OnInspectorGUI()
	{
		iTap = GUILayout.Toolbar(iTap, taps);
		if (iTap == 0) Info();
		if (iTap == 1) Animation();
		if (iTap == 2) Costume();
	}



	void Info() 
	{
		if (EditorGUIService.DrawHeader("Info", "Info", false, false))
		{
			EditorGUIService.BeginContents(false);
			{
				pet.info.ID = EditorGUILayout.TextField("ID", pet.info.ID);
				pet.info.Thumbnail = (Texture) EditorGUILayout.ObjectField("Thumbnail", pet.info.Thumbnail , typeof (Texture));
			}
			EditorGUIService.EndContents();
		}


		if (EditorGUIService.DrawHeader("Body", "Body", false, false))
		{
			EditorGUIService.BeginContents(false);
			{

				pet.body.direction = (Pet.Direction)EditorGUILayout.EnumPopup("direction", pet.body.direction);
				pet.body.root = (Transform)EditorGUILayout.ObjectField("root", pet.body.root, typeof(Transform));
				pet.body.head = (Transform)EditorGUILayout.ObjectField("head", pet.body.head, typeof(Transform));
				pet.body.talk = (Transform)EditorGUILayout.ObjectField("talk", pet.body.talk, typeof(Transform));
				//pet.body.glasses = (Transform)EditorGUILayout.ObjectField("glasses", pet.body.glasses, typeof(Transform));
				//pet.body.eyes[0] = (Transform)EditorGUILayout.ObjectField("eyes 1", pet.body.eyes[0], typeof(Transform));
				//pet.body.eyes[1] = (Transform)EditorGUILayout.ObjectField("eyes 2", pet.body.eyes[1], typeof(Transform));
				//pet.body.mouth = (Transform)EditorGUILayout.ObjectField("mouth", pet.body.mouth, typeof(Transform));
				pet.body.bodycenter = (Transform)EditorGUILayout.ObjectField("body center", pet.body.bodycenter, typeof(Transform));
				//pet.body.foot = (Transform)EditorGUILayout.ObjectField("foot", pet.body.foot, typeof(Transform));
			
			}
			EditorGUIService.EndContents();
		}
		if (EditorGUIService.DrawHeader("Joints", "Joints", false, false))
		{
			EditorGUIService.BeginContents(false);
			{

				if (GUILayout.Button("InitJoint")) 
				{
					pet.joints = new List<PetObj.PetJoint>();
					foreach (var bone in Utility.GameObj.GetAllNode(pet.body.root))
					{
						var joint = new PetObj.PetJoint();
						joint.root = bone.transform;
						joint.position = bone.transform.localPosition;
						joint.rotate = bone.transform.localRotation;
						pet.joints.Add(joint);
					}
					Debug.Log($"Joint : {pet.joints.Count}");
				}
				if (GUILayout.Button("ResetJoint"))
				{
					foreach (var joint in pet.joints)
					{
						joint.root.transform.localPosition = joint.position;
						joint.root.transform.localRotation = joint.rotate;
					}
				}
	

			}
			EditorGUIService.EndContents();
		}

		if (EditorGUIService.DrawHeader("Voice", "Voice", false, false))
		{
			EditorGUIService.BeginContents(false);
			{
				
			}
			EditorGUIService.EndContents();
		}
	}
	void Animation() 
	{
		if (GUILayout.Button("Sleep")) { pet.anim.OnAnimForce(PetAnim.AnimState.Idle); }
		if (GUILayout.Button("SubIdle")) { pet.anim.OnAnimState(1); }


		if (GUILayout.Button("Bad")) { pet.anim.OnAnimForce(PetAnim.AnimState.Bad); }
		if (GUILayout.Button("BallKick")) { pet.anim.OnAnimForce(PetAnim.AnimState.BallKick); }
		if (GUILayout.Button("Dance")) { pet.anim.OnAnimForce(PetAnim.AnimState.Dance); }
		if (GUILayout.Button("Eat")) { pet.anim.OnAnimForce(PetAnim.AnimState.Eat); }
		if (GUILayout.Button("GoodJob")) { pet.anim.OnAnimForce(PetAnim.AnimState.GoodJob); }
		if (GUILayout.Button("LikeLove")) { pet.anim.OnAnimForce(PetAnim.AnimState.LikeLove); }
		if (GUILayout.Button("Need")) { pet.anim.OnAnimForce(PetAnim.AnimState.Need); }
		if (GUILayout.Button("Petting")) { pet.anim.OnAnimForce(PetAnim.AnimState.Petting); }
		if (GUILayout.Button("Walk")) { pet.anim.OnAnimForce(PetAnim.AnimState.Walk); }
		if (GUILayout.Button("Sleep")) { pet.anim.OnAnimForce(PetAnim.AnimState.Sleep); }



		EditorGUILayout.Space(10);
		if (GUILayout.Button("Reset")) { pet.anim.OnReset(); }
	}
	void Costume() {


		// Init first time achievements == null
		if (pet.achievements == null || pet.achievements.Count == 0) {
			pet.achievements = new List<Achievement.AchievementData>();
			5.Loop((i) =>
			{
				pet.achievements.Add(new Achievement.AchievementData()
				{
					Index = i
				});
			});
		}


		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Achievements");
		foreach (var ahv in pet.achievements) 
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.IntField(ahv.Index,GUILayout.Width(40));
			ahv.Name = EditorGUILayout.TextField(ahv.Name, GUILayout.Width(90));
			ahv.RewardType = (Achievement.RewardType)EditorGUILayout.EnumPopup(ahv.RewardType, GUILayout.Width(80));
			if (ahv.RewardType == Achievement.RewardType.Animation) 
			{
				ahv.Clip = (AnimationClip)EditorGUILayout.ObjectField(ahv.Clip , typeof(AnimationClip));
			}
			if (ahv.RewardType == Achievement.RewardType.Costume)
			{
				ahv.Icon = (Sprite)EditorGUILayout.ObjectField(ahv.Icon, typeof(Sprite));
				ahv.Costume = (Transform)EditorGUILayout.ObjectField(ahv.Costume, typeof(Transform));
			}
			EditorGUILayout.EndHorizontal();
		}


		void show(bool ishow) 
		{
			foreach (var ahv in pet.achievements)
			{
				if (ahv.RewardType == Achievement.RewardType.Costume) 
				{
					ahv.Costume.gameObject.SetActive(ishow);
				}
			}
		}


		EditorGUILayout.Space(10);
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("ShowAll"))
		{
			show(true);
		}
		if (GUILayout.Button("HideAll"))
		{
			show(false);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space(20);





	}






}
#endif


