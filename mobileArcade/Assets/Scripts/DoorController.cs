using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public List<GameObject> points;
    public Transform doorCameraFocus;
    public Animator anim;
    GameObject closestPoint;
    GameObject targetPoint;

    public void OpenDoor(PlayerController player)
    {
        ChooseClosestPoint(player.gameObject);
        StartCoroutine("MovePlayerToPoint", player);
        player.InControl(false);
        player.KinematicRigidbody(true);
    }

    void ChooseClosestPoint(GameObject player)
    {
        float distance = 100;
        for (int i = 0; i < 2; i++)
        {
            if (Vector3.Distance(points[i].transform.position, player.transform.position) < distance)
            {
                distance = Vector3.Distance(points[i].transform.position, player.transform.position);
                closestPoint = points[i];
            }
        }
        foreach (GameObject go in points)
        {
            if (go != closestPoint)
                targetPoint = go;
        }
    }

    IEnumerator MovePlayerToPoint(PlayerController player)
    {
        float elapsedTime = 0;
        float time = 1f;

        while (elapsedTime < time)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, closestPoint.transform.position, elapsedTime / time);

            player.transform.localRotation = Quaternion.Lerp(player.transform.localRotation, Quaternion.LookRotation(doorCameraFocus.transform.position - player.camHolder.transform.position), elapsedTime / time);
            player.camHolder.transform.localRotation = Quaternion.Lerp(player.camHolder.transform.localRotation, Quaternion.LookRotation(doorCameraFocus.transform.position - player.camHolder.transform.position), elapsedTime / time);
            player.transform.localEulerAngles = new Vector3(0, player.transform.localEulerAngles.y, 0);
            player.camHolder.transform.localEulerAngles = new Vector3(player.camHolder.transform.localEulerAngles.x, 0, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //player.transform.SetParent(transform);
        anim.SetTrigger(closestPoint.name);

        yield return new WaitForSeconds(0.8f);

        float newElapsedTime = 0;
        float newTime = 1f;
        while (newElapsedTime < newTime)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, targetPoint.transform.position, newElapsedTime / newTime);

            player.transform.localRotation = Quaternion.Lerp(player.transform.localRotation, Quaternion.LookRotation(targetPoint.transform.position - closestPoint.transform.position), newElapsedTime / newTime);
            player.camHolder.transform.localRotation = Quaternion.Lerp(player.camHolder.transform.localRotation, Quaternion.LookRotation(targetPoint.transform.position - closestPoint.transform.position), newElapsedTime / newTime);
            player.transform.localEulerAngles = new Vector3(0, player.transform.localEulerAngles.y, 0);
            player.camHolder.transform.localEulerAngles = new Vector3(player.camHolder.transform.localEulerAngles.x, 0, 0);
            newElapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //player.transform.SetParent(null);
        player.StartCoroutine("ResetInteractiveZoneTrigger");
        player.InControl(true);
        player.KinematicRigidbody(false);
    }
}