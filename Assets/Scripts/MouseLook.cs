using UnityEngine;
using System.Collections;

namespace RedCartel {

	/**
	 * A simple mouse Look script that remains consistent across screen types 
	 * and devices. Consistent turning speed is handled by using DPI as a factor
	 * for the turning rate.
	 * 
	 * Uses alt key to look around - Similar to the cardboard editor camera rotate system for the editor.
	 * 
	 * Simply apply this script to a camera or any Transform object that is 
	 * controlling the camera.
	 * 
	 * This script has no dependencies.
	 * 
	 * © Copyright Red Cartel, 2015. 
	 * Author(s): 
	 * 	Mark Mulligan
	 */
	public class MouseLook : MonoBehaviour {

		public float speed = 1f;

		[Tooltip ("When enabled - Only activates for non-mobile builds. This is because GearVR or Cardboard has it's own system")]
		public bool nonMobileOnly = true;

		#region Private Fields
		private Vector3 lookatRotation = Vector3.zero;

		private float mouseClickX;
		private float mouseClickY;

		private float clickedYaw;
		private float clickedPitch;

		private float yaw;
		private float pitch;

		private float dpiFactor;
		private const float REF_DPI = 72f;
		#endregion

		// Use this for initialization

		void Start () {

			if (Application.isMobilePlatform && nonMobileOnly)
				return;

			//Application.ver
			
			Init ();
		}

		private void Init() 
		{
			dpiFactor = Screen.dpi / REF_DPI;
			UpdateStartDragValues();
		}
		
		// Update is called once per frame
		void Update () 
		{
			if (Application.isMobilePlatform && nonMobileOnly)
				return;

			if (Input.GetKeyDown (KeyCode.LeftAlt)) 
			{
				UpdateStartDragValues();
			}

			if (Input.GetKey (KeyCode.LeftAlt)) 
				UpdateRotation ();
		}

		private void UpdateStartDragValues() 
		{
			mouseClickX = Input.mousePosition.x;
			mouseClickY = Input.mousePosition.y;
			
			clickedYaw = yaw;
			clickedPitch = pitch;
		}

		private void UpdateRotation() {

			// Reset the mouse positions to discard initial mouse offsets from app startup
			float deltaX = Input.mousePosition.x - mouseClickX;
			float deltaY = Input.mousePosition.y - mouseClickY;

			yaw = clickedYaw + (deltaX * speed * dpiFactor);
			pitch = clickedPitch -(deltaY * speed * dpiFactor);

			lookatRotation.y = yaw;
			lookatRotation.x = pitch;

			this.transform.localEulerAngles = lookatRotation;

		}
	}

} // End Namepsace