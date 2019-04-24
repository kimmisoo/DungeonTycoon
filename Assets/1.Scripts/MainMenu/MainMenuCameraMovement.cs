using UnityEngine;
using System;
using System.Collections;

public class MainMenuCameraMovement : MonoBehaviour
{

    Camera cam;
    float i = 0.0f;
    float time = 4.0f;
    Vector3 endPosition;

    // Use this for initialization
    void Start()
    {
        cam = Camera.main;
        endPosition = cam.transform.position + new Vector3(0.0f, -10.0f, 0.0f);

        StartCoroutine(InterpolativeMoveCamera());
    }

    IEnumerator InterpolativeMoveCamera()
    {
        float rate = 1.0f / time;

        float a = Time.fixedTime;
        Debug.Log("start");
        while (i < 1.0f)
        {
            yield return null;
            i += Time.deltaTime * rate;
            cam.transform.position = new Vector3(cam.transform.position.x, Mathf.Lerp(cam.transform.position.y, endPosition.y, rate * Time.deltaTime), cam.transform.position.z);
        }
        float b = Time.fixedTime;
        Debug.Log("end = == = = " + (b - a));
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(SelectCharacter());
    }

    IEnumerator SelectCharacter()
    {
        while (true)
        {
            yield return null;
            if (GameManager.Instance != null && GameManager.Instance.travelers != null && GameManager.Instance.adventurers != null)
            {
                int t = 0;
                if (GameManager.Instance.travelers[t = UnityEngine.Random.Range(0, GameManager.Instance.travelers.Count)] != null)
                {
                    yield return StartCoroutine("ChasingCharacter", GameManager.Instance.travelers[t]);
                }
            }
        }
    }

    IEnumerator ChasingCharacter(GameObject character)
    {
        float startTime = 0.0f;

        startTime = Time.fixedTime;

        while (true)
        {
            yield return null;
            if (character != null && character.GetComponent<Character>().GetCurState() == Character.State.Move && Time.fixedTime - startTime < 5.0f && character.active == true)
            {
                Vector3 t = Vector3.Lerp(cam.transform.position, character.transform.position, 0.3f);
                cam.transform.position = new Vector3(t.x, t.y, cam.transform.position.z);
            }
            else
                break;

        }
    }
}
