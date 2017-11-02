using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CnControls;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1;
    public float lookSpeed = 1;
    public Rigidbody rb;
    public GameObject camHolder;
    public Camera rayCam;
    public Collider coll;
    public bool inControl = true;
    public Animator camAnim;
    public Collider interactiveZoneTrigger;
    bool walking = false;

    // Update is called once per frame
    void Update()
    {
        if (inControl)
        {
            Moving();
            Interacting();
        }
    }

    public void KinematicRigidbody(bool active)
    {
        rb.isKinematic = active;
    }
    public void InControl(bool active)
    {
        inControl = active;
    }
    void Moving()
    {
        //float h = CnInputManager.GetAxis("Horizontal");
        float v = CnInputManager.GetAxis("Vertical");

        var locVel = new Vector3(0, 0, v).normalized;
        /*
            if (locVel.z > 0)
                locVel = new Vector3(locVel.x / 2, locVel.y, locVel.z * moveSpeed);
            else if (locVel.z < 0)
            {
                locVel = new Vector3(locVel.x / 2, locVel.y, locVel.z / 2);
            }
        */

        locVel = new Vector3(locVel.x, locVel.y, locVel.z * moveSpeed);
        rb.velocity = transform.TransformDirection(locVel);

        //rb.velocity = new Vector3(h, 0, v).normalized;

        float hLook = CnInputManager.GetAxis("Horizontal");
        //float vLook = CnInputManager.GetAxis("VerticalLook") * -1;
        //camHolder.transform.Rotate(vLook, 0, 0);
        //camHolder.transform.eulerAngles = new Vector3(camHolder.transform.eulerAngles.x, camHolder.transform.eulerAngles.y, 0);
        rb.angularVelocity = new Vector3(0, hLook * lookSpeed, 0);

        if (rb.velocity.magnitude > 0.2f)
        {
            if (!walking)
            {
                camAnim.CrossFade("camWalk", 0.17f, 0, Random.Range(0f, 1f));
                camAnim.SetBool("Walk", true);
                walking = true;
            }
        }
        else
        {
            if (walking)
            {
                camAnim.CrossFade("camIdle", 0.17f, 0, Random.Range(0f, 1f));
                camAnim.SetBool("Walk", false);
                walking = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9 && other.gameObject.tag == "ActiveObject")
        {
            other.GetComponent<InteractiveController>().ShowIcon(true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9 && other.gameObject.tag == "ActiveObject")
        {
            other.GetComponent<InteractiveController>().ShowIcon(false);
        }
    }
    void Interacting()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = rayCam.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.layer == 9 && hit.collider.gameObject.tag == "ActiveObject")
            {
                InteractiveController ic = hit.collider.gameObject.GetComponent<InteractiveController>();
                if (ic.playerInRange)
                    ic.Interact(this);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = rayCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //if (Physics.Linecast(camHolder.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), out hit))
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.layer == 9 && hit.collider.gameObject.tag == "ActiveObject")
            {
                InteractiveController ic = hit.collider.gameObject.GetComponent<InteractiveController>();
                if (ic.playerInRange)
                    ic.Interact(this);
            }
        }
    }
    public IEnumerator ResetInteractiveZoneTrigger()
    {
        interactiveZoneTrigger.enabled = false;
        yield return new WaitForSecondsRealtime(0.5f);
        interactiveZoneTrigger.enabled = true;
    }
}