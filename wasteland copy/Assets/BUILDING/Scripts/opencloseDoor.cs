using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	public class opencloseDoor : InteractableObject
	{

		public Animator openandclose;
		public bool open;
		public GameObject Player;

		void Start()
		{
			open = false;
	     	Player =GameObject.FindGameObjectWithTag("Player");
		}


    public override void InteractAction()
    {
		open = !open;
       if (open)
		{
			StartCoroutine(opening());
		}else { StartCoroutine(closing());}
	   
    }
    IEnumerator opening()
		{
			print("you are opening the door");
			openandclose.Play("Opening");
			open = true;
			yield return new WaitForSeconds(.5f);
		}

		IEnumerator closing()
		{
			print("you are closing the door");
			openandclose.Play("Closing");
			open = false;
			yield return new WaitForSeconds(.5f);
		}


	}
