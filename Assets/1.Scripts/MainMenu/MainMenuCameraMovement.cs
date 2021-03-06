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
            // 수정요망. null이 아니라 Count를 체크해야하지 않나?
            if (GameManager.Instance != null && GameManager.Instance.travelersDisabled != null && GameManager.Instance.adventurersEnabled != null)
            {
                int t = 0;
                if (GameManager.Instance.travelersDisabled[t = UnityEngine.Random.Range(0, GameManager.Instance.travelersDisabled.Count)] != null)
                {
                    yield return StartCoroutine("ChasingCharacter", GameManager.Instance.travelersDisabled[t]);
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
            if (character != null && Time.fixedTime - startTime < 5.0f && character.active == true)
            {
                Vector3 t = Vector3.Lerp(cam.transform.position, character.transform.position, 0.3f);
                cam.transform.position = new Vector3(t.x, t.y, cam.transform.position.z);
            }
            else
                break;

        }
    }
}
