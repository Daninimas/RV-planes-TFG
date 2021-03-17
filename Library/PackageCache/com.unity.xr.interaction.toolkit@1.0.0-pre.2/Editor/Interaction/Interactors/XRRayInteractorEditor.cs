﻿using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEditor.XR.Interaction.Toolkit
{
    /// <summary>
    /// Custom editor for an <see cref="XRRayInteractor"/>.
    /// </summary>
    [CustomEditor(typeof(XRRayInteractor), true), CanEditMultipleObjects]
    public class XRRayInteractorEditor : XRBaseControllerInteractorEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.maxRaycastDistance"/>.</summary>
        protected SerializedProperty m_MaxRaycastDistance;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.hitDetectionType"/>.</summary>
        protected SerializedProperty m_HitDetectionType;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.sphereCastRadius"/>.</summary>
        protected SerializedProperty m_SphereCastRadius;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.raycastMask"/>.</summary>
        protected SerializedProperty m_RaycastMask;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.raycastTriggerInteraction"/>.</summary>
        protected SerializedProperty m_RaycastTriggerInteraction;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.hoverToSelect"/>.</summary>
        protected SerializedProperty m_HoverToSelect;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.hoverTimeToSelect"/>.</summary>
        protected SerializedProperty m_HoverTimeToSelect;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.enableUIInteraction"/>.</summary>
        protected SerializedProperty m_EnableUIInteraction;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.lineType"/>.</summary>
        protected SerializedProperty m_LineType;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.endPointDistance"/>.</summary>
        protected SerializedProperty m_EndPointDistance;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.endPointHeight"/>.</summary>
        protected SerializedProperty m_EndPointHeight;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.controlPointDistance"/>.</summary>
        protected SerializedProperty m_ControlPointDistance;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.controlPointHeight"/>.</summary>
        protected SerializedProperty m_ControlPointHeight;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.sampleFrequency"/>.</summary>
        protected SerializedProperty m_SampleFrequency;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.velocity"/>.</summary>
        protected SerializedProperty m_Velocity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.acceleration"/>.</summary>
        protected SerializedProperty m_Acceleration;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.additionalFlightTime"/>.</summary>
        protected SerializedProperty m_AdditionalFlightTime;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.referenceFrame"/>.</summary>
        protected SerializedProperty m_ReferenceFrame;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.keepSelectedTargetValid"/>.</summary>
        protected SerializedProperty m_KeepSelectedTargetValid;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.allowAnchorControl"/>.</summary>
        protected SerializedProperty m_AllowAnchorControl;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.useForceGrab"/>.</summary>
        protected SerializedProperty m_UseForceGrab;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.rotateSpeed"/>.</summary>
        protected SerializedProperty m_RotateSpeed;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.translateSpeed"/>.</summary>
        protected SerializedProperty m_TranslateSpeed;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.maxRaycastDistance"/>.</summary>
            public static readonly GUIContent maxRaycastDistance = EditorGUIUtility.TrTextContent("Max Raycast Distance", "Max distance of ray cast. Increase this value will let you reach further.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.sphereCastRadius"/>.</summary>
            public static readonly GUIContent sphereCastRadius = EditorGUIUtility.TrTextContent("Sphere Cast Radius", "Radius of this Interactor's ray, used for sphere casting.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.raycastMask"/>.</summary>
            public static readonly GUIContent raycastMask = EditorGUIUtility.TrTextContent("Raycast Mask", "Layer mask used for limiting raycast targets.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.raycastTriggerInteraction"/>.</summary>
            public static readonly GUIContent raycastTriggerInteraction = EditorGUIUtility.TrTextContent("Raycast Trigger Interaction", "Type of interaction with trigger colliders via raycast.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.hoverToSelect"/>.</summary>
            public static readonly GUIContent hoverToSelect = EditorGUIUtility.TrTextContent("Hover To Select", "If true, this interactor will simulate a Select event if hovered over an Interactable for some amount of time. Selection will be exited when the Interactor is no longer hovering over the Interactable.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.hoverTimeToSelect"/>.</summary>
            public static readonly GUIContent hoverTimeToSelect = EditorGUIUtility.TrTextContent("Hover Time To Select", "Number of seconds for which this interactor must hover over an object to select it.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.enableUIInteraction"/>.</summary>
            public static readonly GUIContent enableUIInteraction = EditorGUIUtility.TrTextContent("Enable Interaction with UI GameObjects", "If checked, this interactor will be able to affect UI.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.lineType"/>.</summary>
            public static readonly GUIContent lineType = EditorGUIUtility.TrTextContent("Line Type", "Line type of the ray cast.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.endPointDistance"/>.</summary>
            public static readonly GUIContent endPointDistance = EditorGUIUtility.TrTextContent("End Point Distance", "Increase this value distance will make the end of curve further from the start point.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.controlPointDistance"/>.</summary>
            public static readonly GUIContent controlPointDistance = EditorGUIUtility.TrTextContent("Control Point Distance", "Increase this value will make the peak of the curve further from the start point.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.endPointHeight"/>.</summary>
            public static readonly GUIContent endPointHeight = EditorGUIUtility.TrTextContent("End Point Height", "Decrease this value will make the end of the curve drop lower relative to the start point.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.controlPointHeight"/>.</summary>
            public static readonly GUIContent controlPointHeight = EditorGUIUtility.TrTextContent("Control Point Height", "Increase this value will make the peak of the curve higher relative to the start point.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.sampleFrequency"/>.</summary>
            public static readonly GUIContent sampleFrequency = EditorGUIUtility.TrTextContent("Sample Frequency", "Gets or sets the number of sample points of the curve, should be at least 3, the higher the better quality.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.velocity"/>.</summary>
            public static readonly GUIContent velocity = EditorGUIUtility.TrTextContent("Velocity", "Initial velocity of the projectile. Increase this value will make the curve reach further.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.acceleration"/>.</summary>
            public static readonly GUIContent acceleration = EditorGUIUtility.TrTextContent("Acceleration", "Gravity of the projectile in the reference frame.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.additionalFlightTime"/>.</summary>
            public static readonly GUIContent additionalFlightTime = EditorGUIUtility.TrTextContent("Additional Flight Time", "Additional flight time after the projectile lands at the same height of the start point in the tracking space. Increase this value will make the end point drop lower in height.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.referenceFrame"/>.</summary>
            public static readonly GUIContent referenceFrame = EditorGUIUtility.TrTextContent("Reference Frame", "The reference frame of the projectile. If not set it will try to find the XRRig GameObject, and if that does not exist it will use its own Transform.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.hitDetectionType"/>.</summary>
            public static readonly GUIContent hitDetectionType = EditorGUIUtility.TrTextContent("Hit Detection Type", "The type of hit detection used to hit interactable objects.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.keepSelectedTargetValid"/>.</summary>
            public static readonly GUIContent keepSelectedTargetValid = EditorGUIUtility.TrTextContent("Keep Selected Target Valid", "Keep selecting the target when not pointing to it after initially selecting it. It is recommended to set this value to true for grabbing objects, false for teleportation interactables.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.allowAnchorControl"/>.</summary>
            public static readonly GUIContent allowAnchorControl = EditorGUIUtility.TrTextContent("Anchor Control", "Allows the user to move the attach anchor point using the joystick.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.useForceGrab"/>.</summary>
            public static readonly GUIContent useForceGrab = EditorGUIUtility.TrTextContent("Force Grab", "Force grab moves the object to your hand rather than interacting with it at a distance.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.rotateSpeed"/>.</summary>
            public static readonly GUIContent rotateSpeed = EditorGUIUtility.TrTextContent("Rotate Speed", "Speed that the anchor is rotated.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.translateSpeed"/>.</summary>
            public static readonly GUIContent translateSpeed = EditorGUIUtility.TrTextContent("Translate Speed", "Speed that the anchor is translated.");
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            m_MaxRaycastDistance = serializedObject.FindProperty("m_MaxRaycastDistance");
            m_HitDetectionType = serializedObject.FindProperty("m_HitDetectionType");
            m_SphereCastRadius = serializedObject.FindProperty("m_SphereCastRadius");
            m_RaycastMask = serializedObject.FindProperty("m_RaycastMask");
            m_RaycastTriggerInteraction = serializedObject.FindProperty("m_RaycastTriggerInteraction");
            m_HoverToSelect = serializedObject.FindProperty("m_HoverToSelect");
            m_HoverTimeToSelect = serializedObject.FindProperty("m_HoverTimeToSelect");
            m_EnableUIInteraction = serializedObject.FindProperty("m_EnableUIInteraction");

            m_LineType = serializedObject.FindProperty("m_LineType");
            m_EndPointDistance = serializedObject.FindProperty("m_EndPointDistance");
            m_EndPointHeight = serializedObject.FindProperty("m_EndPointHeight");
            m_ControlPointDistance = serializedObject.FindProperty("m_ControlPointDistance");
            m_ControlPointHeight = serializedObject.FindProperty("m_ControlPointHeight");
            m_SampleFrequency = serializedObject.FindProperty("m_SampleFrequency");

            m_Velocity = serializedObject.FindProperty("m_Velocity");
            m_Acceleration = serializedObject.FindProperty("m_Acceleration");
            m_AdditionalFlightTime = serializedObject.FindProperty("m_AdditionalFlightTime");
            m_ReferenceFrame = serializedObject.FindProperty("m_ReferenceFrame");

            m_KeepSelectedTargetValid = serializedObject.FindProperty("m_KeepSelectedTargetValid");
            m_AllowAnchorControl = serializedObject.FindProperty("m_AllowAnchorControl");
            m_UseForceGrab = serializedObject.FindProperty("m_UseForceGrab");

            m_RotateSpeed = serializedObject.FindProperty("m_RotateSpeed");
            m_TranslateSpeed = serializedObject.FindProperty("m_TranslateSpeed");

            // Set default expanded for some foldouts
            const string initializedKey = "XRI." + nameof(XRRayInteractorEditor) + ".Initialized";
            if (!SessionState.GetBool(initializedKey, false))
            {
                SessionState.SetBool(initializedKey, true);
                m_LineType.isExpanded = true;
                m_SelectActionTrigger.isExpanded = true;
            }
        }

        /// <inheritdoc />
        protected override void DrawProperties()
        {
            // Not calling base method to completely override drawn properties

            DrawInteractionManagement();

            EditorGUILayout.Space();

            DrawInteractionConfiguration();

            EditorGUILayout.Space();

            DrawRaycastConfiguration();

            EditorGUILayout.Space();

            DrawSelectionConfiguration();
        }

        /// <summary>
        /// Draw the property fields related to interaction configuration.
        /// </summary>
        protected virtual void DrawInteractionConfiguration()
        {
            EditorGUILayout.PropertyField(m_EnableUIInteraction, Contents.enableUIInteraction);
            EditorGUILayout.PropertyField(m_UseForceGrab, Contents.useForceGrab);
            EditorGUILayout.PropertyField(m_AllowAnchorControl, Contents.allowAnchorControl);
            if (m_AllowAnchorControl.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_RotateSpeed, Contents.rotateSpeed);
                    EditorGUILayout.PropertyField(m_TranslateSpeed, Contents.translateSpeed);
                }
            }

            EditorGUILayout.PropertyField(m_AttachTransform, BaseContents.attachTransform);
        }

        /// <summary>
        /// Draw the Raycast Configuration foldout.
        /// </summary>
        /// <seealso cref="DrawRaycastConfigurationNested"/>
        protected virtual void DrawRaycastConfiguration()
        {
            m_LineType.isExpanded = EditorGUILayout.Foldout(m_LineType.isExpanded, EditorGUIUtility.TrTempContent("Raycast Configuration"), true);
            if (m_LineType.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    DrawRaycastConfigurationNested();
                }
            }
        }

        /// <summary>
        /// Draw the nested contents of the Raycast Configuration foldout.
        /// </summary>
        /// <seealso cref="DrawRaycastConfiguration"/>
        protected virtual void DrawRaycastConfigurationNested()
        {
            EditorGUILayout.PropertyField(m_LineType, Contents.lineType);

            using (new EditorGUI.IndentLevelScope())
            {
                switch (m_LineType.enumValueIndex)
                {
                    case (int)XRRayInteractor.LineType.StraightLine:
                        EditorGUILayout.PropertyField(m_MaxRaycastDistance, Contents.maxRaycastDistance);
                        break;
                    case (int)XRRayInteractor.LineType.ProjectileCurve:
                        EditorGUILayout.PropertyField(m_ReferenceFrame, Contents.referenceFrame);
                        EditorGUILayout.PropertyField(m_Velocity, Contents.velocity);
                        EditorGUILayout.PropertyField(m_Acceleration, Contents.acceleration);
                        EditorGUILayout.PropertyField(m_AdditionalFlightTime, Contents.additionalFlightTime);
                        EditorGUILayout.PropertyField(m_SampleFrequency, Contents.sampleFrequency);
                        break;
                    case (int)XRRayInteractor.LineType.BezierCurve:
                        EditorGUILayout.PropertyField(m_EndPointDistance, Contents.endPointDistance);
                        EditorGUILayout.PropertyField(m_EndPointHeight, Contents.endPointHeight);
                        EditorGUILayout.PropertyField(m_ControlPointDistance, Contents.controlPointDistance);
                        EditorGUILayout.PropertyField(m_ControlPointHeight, Contents.controlPointHeight);
                        EditorGUILayout.PropertyField(m_SampleFrequency, Contents.sampleFrequency);
                        break;
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_RaycastMask, Contents.raycastMask);
            EditorGUILayout.PropertyField(m_RaycastTriggerInteraction, Contents.raycastTriggerInteraction);
            EditorGUILayout.PropertyField(m_HitDetectionType, Contents.hitDetectionType);
            if (m_HitDetectionType.enumValueIndex == (int)XRRayInteractor.HitDetectionType.SphereCast)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_SphereCastRadius, Contents.sphereCastRadius);
                }
            }
        }

        /// <summary>
        /// Draw the Selection Configuration foldout.
        /// </summary>
        /// <seealso cref="DrawSelectionConfigurationNested"/>
        protected virtual void DrawSelectionConfiguration()
        {
            m_SelectActionTrigger.isExpanded = EditorGUILayout.Foldout(m_SelectActionTrigger.isExpanded, EditorGUIUtility.TrTempContent("Selection Configuration"), true);
            if (m_SelectActionTrigger.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    DrawSelectionConfigurationNested();
                }
            }
        }

        /// <summary>
        /// Draw the nested contents of the Selection Configuration foldout.
        /// </summary>
        /// <seealso cref="DrawSelectionConfiguration"/>
        protected virtual void DrawSelectionConfigurationNested()
        {
            DrawSelectActionTrigger();
            EditorGUILayout.PropertyField(m_KeepSelectedTargetValid, Contents.keepSelectedTargetValid);
            EditorGUILayout.PropertyField(m_HideControllerOnSelect, BaseControllerContents.hideControllerOnSelect);
            EditorGUILayout.PropertyField(m_HoverToSelect, Contents.hoverToSelect);
            if (m_HoverToSelect.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_HoverTimeToSelect, Contents.hoverTimeToSelect);
                }
            }
            EditorGUILayout.PropertyField(m_StartingSelectedInteractable, BaseContents.startingSelectedInteractable);
        }
    }
}
