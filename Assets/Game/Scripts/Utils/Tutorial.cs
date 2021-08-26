using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Tutorial : MonoBehaviour
{
    [Header("Tutorial data")]
    [SerializeField]
    Material tutorialMaterial;
    [SerializeField]
    GameObject tutorialText;
    [SerializeField]
    Vector3 textPositionOffset;
    [SerializeField]
    PlaneControllerPlayer planeData;
    [SerializeField]
    Transform textFacingPosition;

    [Space]
    [Header("Tutorial Objects")]
    [SerializeField]
    GameObject mixtureControl;
    [SerializeField]
    GameObject planeJoystick;
    [SerializeField]
    GameObject yawPosition;
    [SerializeField]
    GameObject[] playerHands;
    [SerializeField]
    List<GameObject> rings;
    [SerializeField]
    GameObject tanksParent;

    int state = 0;
    List<GameObject> greenObjects = new List<GameObject>(); // Los objetos que pintamos de verde para el usuario
    int lastState = -1;
    bool changinState = false;
    bool yawRight = false;
    bool yawLeft = false;
    Transform textCanvas;

    private void Start()
    {
        textCanvas = tutorialText.transform.parent.transform;

        changeState(); // To set the first state

        foreach(GameObject ring in rings)
        {
            ring.SetActive(false);
        }

        tanksParent.SetActive(false);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        updateTextRotation();

        if(!changinState)
            doStateSpecificActions();
    }

    void updateTextRotation()
    {
        textCanvas.localPosition = textPositionOffset;
        textCanvas.LookAt(textFacingPosition);
    }

    void doStateSpecificActions()
    {
        switch (state)
        {
            case 0: // Mixture control

                if(planeData.mixtureControlNormal > 0.9)
                {
                    ++state;
                    Invoke("changeState", 0.2f);
                    changinState = true;
                }
                break;


            case 1: // Joystick

                if (planeData.joystickNormal != new Vector2(0f, 0f))
                {
                    ++state;
                    Invoke("changeState", 3f);
                    changinState = true;
                }
                break;

            case 2: // Yaw
                if(planeData.rotateYawNormal == -1)
                {
                    yawLeft = true;
                }
                else if(planeData.rotateYawNormal == 1)
                {
                    yawRight = true;
                }

                if(yawLeft && yawRight)
                {
                    ++state;
                    Invoke("changeState", 0.2f);
                    changinState = true;
                }
                break;

            case 3:
                if(rings.Count > 0)
                {
                    if (rings[0].GetComponent<Ring>().collided)
                    {
                        Destroy(rings[0]);
                        rings.RemoveAt(0);
                        rings[0].SetActive(true);
                    }
                }
                else
                {
                    ++state;
                    Invoke("changeState", 0.2f);
                    changinState = true;
                }
                break;

            case 4:
                if (planeData.shooting)
                {
                    ++state;
                    Invoke("changeState", 1f);
                    changinState = true;
                }
                break;

            case 5:
                if (tanksParent.transform.childCount >= 0)
                {
                    ++state;
                    Invoke("changeState", 1f);
                    changinState = true;
                }
                break;
        }
    }


    void changeState()
    {
        if (state != lastState)
        {
            deleteAllTutorialMaterial();
            changinState = false;

            switch (state)
            { // Para añadir las nuevas coas al cambiar de estado
                case 0: // Mixture control
                    tutorialText.GetComponent<TMPro.TextMeshProUGUI>().text = "Pick and push the Mixture Control to accelerate the plane";

                    Collider mixtureCtrlCollider = mixtureControl.GetComponent<XRSimpleInteractable>().colliders[0];
                    textCanvas.parent = mixtureCtrlCollider.transform;

                    greenObjects.Add(mixtureCtrlCollider.gameObject);
                    setAllTutorialMaterial();
                    break;

                case 1: // Joystick
                    tutorialText.GetComponent<TMPro.TextMeshProUGUI>().text = "Pick and move the Joystick to control the plane";

                    Collider joystickCtrlCollider = planeJoystick.GetComponent<XRSimpleInteractable>().colliders[0];
                    textCanvas.parent = joystickCtrlCollider.transform;

                    greenObjects.Add(joystickCtrlCollider.gameObject);
                    setAllTutorialMaterial();
                    break;

                case 2: // guiñada
                    tutorialText.GetComponent<TMPro.TextMeshProUGUI>().text = "Press X or A to make the Yaw turn";
                    textCanvas.parent = yawPosition.transform;

                    yawRight = false;
                    yawLeft = false;

                    /*foreach(GameObject hand in playerHands)
                    {
                        greenObjects.Add(hand.transform.GetChild(0).gameObject);
                    }
                    setAllTutorialMaterial();*/
                    break;

                case 3: // Ir a los aros
                    tutorialText.GetComponent<TMPro.TextMeshProUGUI>().text = "Go through the rings!";
                    textCanvas.parent = yawPosition.transform;

                    rings[0].SetActive(true);
                    break;

                case 4: // disparar
                    tutorialText.GetComponent<TMPro.TextMeshProUGUI>().text = "Press the trigger while holding the joystick to SHOOT";
                    textCanvas.parent = yawPosition.transform;

                    greenObjects.Add(planeJoystick.GetComponent<XRSimpleInteractable>().colliders[0].gameObject);
                    setAllTutorialMaterial();
                    break;

                case 5: // disparar a los tanques
                    tutorialText.GetComponent<TMPro.TextMeshProUGUI>().text = "Shoot the enemy tanks!";
                    textCanvas.parent = yawPosition.transform;

                    tanksParent.SetActive(true);
                    break;
            }
        }

        lastState = state;
    }

    void setAllTutorialMaterial()
    {
        foreach(GameObject tutorialObject in greenObjects) {
            // Le añade un material extra para que se renderize con el último
            MeshRenderer mesh = tutorialObject.GetComponent<MeshRenderer>();

            if (mesh.materials.Length < 2)
            {
                List<Material> list = new List<Material>(mesh.materials);
                list.Add(tutorialMaterial);

                mesh.materials = list.ToArray();
            }
        }
    }

    void deleteAllTutorialMaterial()
    {
        foreach (GameObject tutorialObject in greenObjects)
        {
            if (tutorialObject != null)
            {
                // Le quita el último material
                MeshRenderer mesh = tutorialObject.GetComponent<MeshRenderer>();

                if (mesh.materials.Length > 1)
                {
                    List<Material> list = new List<Material>(mesh.materials);
                    list.RemoveAt(list.Count - 1);

                    mesh.materials = list.ToArray();
                }
            }
        }

        greenObjects.Clear();
    }
}
