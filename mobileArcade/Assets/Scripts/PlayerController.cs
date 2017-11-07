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
    public SphereCollider interactiveZoneTrigger;
    bool walking = false;

    public Animator blinkAnim;
    float blinkCooldown = 0;
    public Animator toDoListAnim;
    bool toDoListActive = false;

    // Update is called once per frame
    void Update()
    {
        if (inControl && !toDoListActive)
        {
            Moving();
            Interacting();
            Blinking();
        }
    }

    void Blinking()
    {
        if (blinkCooldown > 0)
            blinkCooldown -= Time.deltaTime;
        else
        {
            blinkCooldown = Random.Range(0.1f, 7f);
            blinkAnim.SetTrigger("Blink");
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
            StartCoroutine("InteractTouch");
        }
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine("InteractMouse");
        }
    }
    public IEnumerator InteractTouch()
    {
        float time = 0;
        while (Input.GetTouch(0).phase != TouchPhase.Ended && time < 0.2f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        if (Input.GetTouch(0).phase == TouchPhase.Ended && time < 0.2f)
        {
            Ray ray = rayCam.ScreenPointToRay(Input.GetTouch(0).position);
            Interact(ray, false);
        }
    }
    public IEnumerator InteractMouse()
    {
        yield return new WaitForSeconds(0.2f);
        if (!Input.GetMouseButton(0))
        {
            Ray ray = rayCam.ScreenPointToRay(Input.mousePosition);

            Interact(ray, false);
        }
    }
    void Interact(Ray _ray, bool castUI)
    {
        RaycastHit hit;
        if (Physics.Raycast(_ray, out hit))
        {
            if (hit.collider.gameObject.layer == 9 && hit.collider.gameObject.tag == "ActiveObject")
            {
                InteractiveController ic = hit.collider.gameObject.GetComponent<InteractiveController>();
                if (ic.playerInRange)
                    ic.Interact(this);
            }
            else if (hit.collider.gameObject.layer == 5 && hit.collider.gameObject.name == "ToDoList")
            {
                toDoListAnim.SetBool("Active", true);
            }
        }
    }
    public IEnumerator ResetInteractiveZoneTrigger()
    {
        interactiveZoneTrigger.enabled = false;
        yield return new WaitForEndOfFrame();
        interactiveZoneTrigger.enabled = true;
    }
    public void ToDoListActive()
    {
        if (inControl)
        {
            toDoListActive = true;
            toDoListAnim.SetBool("Active", true);
        }
    }
    public void ToDoListInactive()
    {
        toDoListActive = false;
        toDoListAnim.SetBool("Active", false);
    }
}