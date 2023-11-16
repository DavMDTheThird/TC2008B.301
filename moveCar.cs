using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class moveCar : MonoBehaviour{
    GameObject[] wheels = new GameObject[4];


    [SerializeField] GameObject wheel_prefav;
    [SerializeField] GameObject car;
    [SerializeField] Vector3 displacement;
    [SerializeField] float angle;
    [SerializeField] AXIS rotationAxis;


    Mesh[] mesh = new Mesh[4];
    Mesh car_mesh;
    Vector3[][] baseVertices = new Vector3[4][];
    Vector3[] car_baseVertices;
    Vector3[][] newVertices = new Vector3[4][];
    Vector3[] car_newVertices;
    Vector3[] wheel_scale = new Vector3[4];
    Vector3 car_scale;

    Vector3[] wheelsPos = new Vector3[4];

    // Start is called before the first frame update
    void Start(){
        wheelsPos[0] = new Vector3(4.8f, 2.6f, -10f);
        wheelsPos[1] = new Vector3(-4.8f, 2.6f, -10f);
        wheelsPos[2] = new Vector3(4.8f, 2.6f, 7.6f);
        wheelsPos[3] = new Vector3(-4.8f, 2.6f, 7.6f);

        for(int i = 0; i < wheels.Length; i++){
            wheels[i] = Instantiate(wheel_prefav, new Vector3(0f, 0f, 0f), Quaternion.identity);

            // Crea el array de wheels, mesh y baseVertices (y scalea las llantas)
            mesh[i] = wheels[i].GetComponentInChildren<MeshFilter>().mesh;
            baseVertices[i] = mesh[i].vertices;
            wheels[i].transform.localScale = new Vector3(2f, 2f, 2f);
            wheel_scale[i] = wheels[i].transform.localScale;
            // Debug.Log(wheel_scale[i]);

            // Crea el array de newVertices que contiene a todas las llantas
            newVertices[i] = new Vector3 [baseVertices[i].Length];
            for (int k = 0; k <baseVertices[i].Length; ++k){
                newVertices[i][k] = baseVertices[i][k];
            }   
        }
        car_scale = transform.localScale;
        // Debug.Log(car_scale.x);
        car_mesh = GetComponentInChildren<MeshFilter>().mesh;
        car_baseVertices = car_mesh.vertices;

        // Create a copy of the original vertices
        car_newVertices = new Vector3 [car_baseVertices.Length];
        for (int i = 0; i <car_baseVertices.Length; ++i){
            car_newVertices[i] = car_baseVertices[i];
        }
    }

    // Update is called once per frame
    void Update(){
        DoTransform_Car(displacement, rotationAxis, car_baseVertices, car_newVertices, car_mesh, car_scale);
        for(int i = 0; i < wheels.Length; i++){
            DoTransform_Wheels(wheelsPos[i], displacement, rotationAxis, baseVertices[i], newVertices[i], mesh[i], wheel_scale[i]);
        }
    }

    void DoTransform_Wheels(Vector3 wheelsPos, Vector3 displacement, AXIS rotationAxis, Vector3[] baseVertices, Vector3[] newVertices, Mesh mesh, Vector3 scale){
        //----------------------------------------------------------------------------------
        Matrix4x4 move = HW_Transforms.TranslationMat(displacement.x / scale.x * Time.time,
                                                      displacement.y / scale.y * Time.time,
                                                      displacement.z / scale.z * Time.time);

        Matrix4x4 rotate = HW_Transforms.RotateMat(angle * Time.time, rotationAxis);

        Matrix4x4 posOrigin = HW_Transforms.TranslationMat(-displacement.x,
                                                           -displacement.y,
                                                           -displacement.z);

        Matrix4x4 posObject = HW_Transforms.TranslationMat(wheelsPos.x / scale.x,
                                                           wheelsPos.y / scale.y,
                                                           wheelsPos.z / scale.z);

        Matrix4x4 composite = posObject * move  * rotate;

        for (int i = 0; i < newVertices.Length; i++){
            Vector4 temp = new Vector4(baseVertices[i].x,
                                       baseVertices[i].y, 
                                       baseVertices[i].z,
                                       1);

            newVertices[i] = composite * temp;
        }
        //----------------------------------------------------------------------------------
        

        // Replace the vertices in the mesh
        mesh.vertices = newVertices;
        mesh.RecalculateNormals();
    }

    void DoTransform_Car(Vector3 car_displacement, AXIS car_rotationAxis, Vector3[] car_baseVertices, Vector3[] car_newVertices, Mesh car_mesh, Vector3 car_scale){
        //----------------------------------------------------------------------------------
        Matrix4x4 move = HW_Transforms.TranslationMat(car_displacement.x / car_scale.x * Time.time,
                                                      car_displacement.y / car_scale.y * Time.time,
                                                      car_displacement.z / car_scale.z * Time.time);

        Matrix4x4 rotate = HW_Transforms.RotateMat(angle * Time.time, car_rotationAxis);

        Matrix4x4 posOrigin = HW_Transforms.TranslationMat(-car_displacement.x,
                                                           -car_displacement.y,
                                                           -car_displacement.z);

        Matrix4x4 posObject = HW_Transforms.TranslationMat(car_displacement.x,
                                                           car_displacement.y,
                                                           car_displacement.z);

        Matrix4x4 composite = move;

        for (int i = 0; i < car_newVertices.Length; i++){
            Vector4 temp = new Vector4(car_baseVertices[i].x,
                                       car_baseVertices[i].y, 
                                       car_baseVertices[i].z,
                                       1);

            car_newVertices[i] = composite * temp;
        }
        //----------------------------------------------------------------------------------

        // Replace the vetices in the car_mesh
        car_mesh.vertices = car_newVertices;
        car_mesh.RecalculateNormals();
    }
}
