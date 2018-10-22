using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.ARMobile;
using WaveEngine.ARMobile.Components;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace XamarinForms3DCarSample
{
    [DataContract]
    public class PlaneDetector : Behavior
    {
        [RequiredService]
        protected Input input;

        [RequiredComponent]
        protected ARMobileProvider arProvider;

        private Guid lastPlaneId;

        private bool dirtyTarget;

        private Transform3D targetTransform;

        [DataMember]
        private string targetEntity;

        /// <summary>
        /// Gets or sets the target entity path
        /// </summary>
        [RenderPropertyAsEntity(
            new string[] { "WaveEngine.Framework.Graphics.Transform3D" },
            CustomPropertyName = "Target Entity",
            Tooltip = "The target entity to be placed")]
        public string TargetEntity
        {
            get
            {
                return this.targetEntity;
            }

            set
            {
                this.targetEntity = value;
                this.dirtyTarget = true;
            }
        }

        [DataMember]
        [RenderProperty]
        public ARMobileHitType HitType { get; set; }

        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.HitType = ARMobileHitType.ExistingPlane;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.GetEntityTransform(this.targetEntity, out this.targetTransform);
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.dirtyTarget)
            {
                this.dirtyTarget = false;
                this.GetEntityTransform(this.targetEntity, out this.targetTransform);
            }

            if (this.targetTransform == null)
            {
                return;
            }

            var newTouches = this.input.TouchPanelState
                                       .Where(t => t.IsNew);

            foreach (var touch in newTouches)
            {
                ARMobileHitTestResult[] results;
                if (this.arProvider.HitTest(touch.Position, this.HitType, out results))
                {
                    var firstResult = results.First();

                    if (firstResult.Anchor == null)
                    {
                        continue;
                    }

                    var resultAnchorId = firstResult.Anchor.Id;
                    var worldTransform = firstResult.WorldTransform;

                    if (this.lastPlaneId != resultAnchorId)
                    {
                        this.PlaceItem(worldTransform);
                    }

                    //this.PlaySound();

                    this.lastPlaneId = resultAnchorId;
                    break;
                }
            }
        }

        private void GetEntityTransform(string entityPath, out Transform3D entityTransform)
        {
            if (entityPath != null)
            {
                var target = this.EntityManager.Find(entityPath, this.Owner);
                if (target != null)
                {
                    if (!WaveServices.Platform.IsEditor)
                    {
                        target.IsVisible = false;
                    }

                    entityTransform = target.FindComponent<Transform3D>();
                }
                else
                {
                    entityTransform = null;
                }
            }
            else
            {
                entityTransform = null;
            }
        }

        private void PlaceItem(Matrix t)
        {
            this.targetTransform.Position = t.Translation;
            this.targetTransform.Orientation = t.Orientation;
            this.targetTransform.Owner.IsVisible = true;

            ////var soundEmitter = this.targetTransform.Owner.FindComponentsInChildren<SoundEmitter3D>().First();

            ////if (soundEmitter.IsMuted)
            ////{
            ////    soundEmitter.Play();
            ////    soundEmitter.IsMuted = false;
            ////}
        }
    }
}
