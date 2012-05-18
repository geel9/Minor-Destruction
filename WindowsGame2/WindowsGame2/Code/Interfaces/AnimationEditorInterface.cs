using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiningGame.Code.Managers;
using MiningGame.Code.Structs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiningGame.Code.Server;
using YogUILibrary.UIComponents;
using YogUILibrary.Managers;
using MiningGame.Code.Entities;
namespace MiningGame.Code.Interfaces
{
    class AnimationEditorInterface : Interface
    {
        public List<AnimationControlPointUI> controlPoints = new List<AnimationControlPointUI>();

        public Animateable curAnimateable;

        public Button newAnimationButton;
        public Button openAnimationButton;
        public Button saveAnimationButton;
        public Button backButton;
        public Button startLoopingButton;
        public Button stopLoopingButton;

        public ListBox framesListBox;
        public Button addFrameButton;
        public Button delFrameButton;

        public TextField textFieldframeName;
        public TextField textFieldFrameAsset;
        public TextField textFieldStartLooping;
        public TextField textFieldEndLooping;
        public TextField textFieldFrameDelay;

        public Button moveUpButton;
        public Button moveDownButton;

        private float imgScale = 1;

        public void Zoom(float amount)
        {
            if (amount == 0) return;
            float oldScale = imgScale;
            imgScale += amount;
            if (imgScale < 1) imgScale = 1;
            if (imgScale > 10) imgScale = 10;
            foreach (AnimationControlPointUI cp in controlPoints)
            {
                cp.scale = imgScale;
            }
        }

        public AnimationEditorInterface()
        {
            InputManager.BindMouse(() =>
            {
                Zoom(-.5f);
            }, MouseButton.Scrolldown);

            InputManager.BindMouse(() =>
            {
                Zoom(.5f);
            }, MouseButton.Scrollup);

            InputManager.BindMouse(() =>
            {
                if (framesListBox.selectedIndex != -1)
                    controlPoints.Add(new AnimationControlPointUI((InputManager.GetMousePosV() - Main.Center), "CP", imgScale));
            }, MouseButton.Middle);

            InputManager.BindKey(() =>
            {
                int index = -1;
                for (int i = 0; i < controlPoints.Count; i++)
                {
                    if (controlPoints[i].selected)
                    {
                        index = i;
                        break;
                    }
                }
                if (index != -1)
                {
                    controlPoints.RemoveAt(index);
                }
            }, Microsoft.Xna.Framework.Input.Keys.Delete);

            curAnimateable = new Animateable();
            Vector2 start = new Vector2(0, 0);
            start.X += 40;
            start.Y += 22;

            newAnimationButton = new Button(start, "New", AssetManager.GetFont("Console"), newAnimation);
            start.X += 120;

            openAnimationButton = new Button(start, "Open", AssetManager.GetFont("Console"), openAnimation);


            start.X += 120;
            saveAnimationButton = new Button(start, "Save", AssetManager.GetFont("Console"), saveAnimation);

            start.X += 120;
            backButton = new Button(start, "Back", AssetManager.GetFont("Console"), back);

            start.X = 5;
            start.Y = 65;

            framesListBox = new ListBox(start, 150, 340, AssetManager.GetFont("Console"), listBoxFrames_SelectedIndexChanged);
            framesListBox.autoAdjust = false;

            framesListBox.dataSource = new List<string>(new string[] { "Frames" });

            start.X = 42f;
            start.Y = Main.graphics.PreferredBackBufferHeight - 60;
            addFrameButton = new Button(start, "Add", AssetManager.GetFont("Console"), addFrame);
            start.X += 76;
            delFrameButton = new Button(start, "Del", AssetManager.GetFont("Console"), delFrame);

            start.Y += 30;
            moveDownButton = new Button(start, "Up", AssetManager.GetFont("Console"), moveUp);
            start.X -= 76;
            moveUpButton = new Button(start, "Down", AssetManager.GetFont("Console"), moveDown);


            start.X = Main.graphics.PreferredBackBufferWidth - 220;
            start.Y = 65;
            textFieldFrameAsset = new TextField(start, 200, 20, Color.Black, AssetManager.GetFont("Console"), (string s) => { }, (string s) => { textBoxesChanged(); });
            start.Y += 40;
            textFieldframeName = new TextField(start, 200, 20, Color.Black, AssetManager.GetFont("Console"), (string s) => { }, (string s) => { textBoxesChanged(); });

            start.Y += 40;
            textFieldFrameDelay = new TextField(start, 100, 20, Color.Black, AssetManager.GetFont("Console"), (string s) => { }, (string s) => { textBoxesChanged(); });



            start.Y += 100;
            textFieldStartLooping = new TextField(start, 200, 20, Color.Black, AssetManager.GetFont("Console"), null);
            start.Y += 40;
            textFieldEndLooping = new TextField(start, 200, 20, Color.Black, AssetManager.GetFont("Console"), null);

            start.Y += 40;
            start.X += 45;
            startLoopingButton = new Button(start, "Start", AssetManager.GetFont("Console"), () => { curAnimateable.StartLooping(textFieldStartLooping.GetText(), textFieldEndLooping.GetText()); });
            start.X += 110;
            stopLoopingButton = new Button(start, "Stop", AssetManager.GetFont("Console"), curAnimateable.StopLooping);

            textFieldframeName.placeHolderText = "Frame name";
            textFieldFrameAsset.placeHolderText = "Frame asset";
            textFieldStartLooping.placeHolderText = "Loop start";
            textFieldEndLooping.placeHolderText = "Loop end";
            textFieldFrameDelay.placeHolderText = "Frame delay";

            recomputeListBox();

            base.blocking = false;
        }

        public List<AnimationControlPoint> CpUiToCp(List<AnimationControlPointUI> cp)
        {
            List<AnimationControlPoint> ret = new List<AnimationControlPoint>();
            foreach (AnimationControlPointUI c in cp)
            {
                ret.Add(new AnimationControlPoint(c.tf.GetText(), c.position.X, c.position.Y));
            }
            return ret;
        }

        public List<AnimationControlPointUI> CpToCpUi(List<AnimationControlPoint> cp)
        {
            List<AnimationControlPointUI> ret = new List<AnimationControlPointUI>();
            foreach (AnimationControlPoint c in cp)
            {
                AnimationControlPointUI a = new AnimationControlPointUI(new Vector2(c.x, c.y), c.name, 1f);
                a.scale = imgScale;
                ret.Add(a);
            }
            return ret;
        }

        public void back()
        {
            destroy();
            StartGameInterface se = new StartGameInterface();
        }

        public void moveDown()
        {
            int index = framesListBox.selectedIndex;
            if (index >= 0 && index < curAnimateable.CurAnimation.numberFrames - 1)
            {
                AnimationFrame below = curAnimateable.CurAnimation.frames[index + 1];
                AnimationFrame current = curAnimateable.CurAnimation.frames[index];
                curAnimateable.CurAnimation.frames[index + 1] = current;
                curAnimateable.CurAnimation.frames[index] = below;
                framesListBox.selectedIndex++;
                recomputeListBox();
            }
        }

        public void moveUp()
        {
            int index = framesListBox.selectedIndex;
            if (index >= 1 && index < curAnimateable.CurAnimation.numberFrames)
            {
                AnimationFrame above = curAnimateable.CurAnimation.frames[index - 1];
                AnimationFrame current = curAnimateable.CurAnimation.frames[index];
                curAnimateable.CurAnimation.frames[index - 1] = current;
                curAnimateable.CurAnimation.frames[index] = above;
                framesListBox.selectedIndex--;
                recomputeListBox();
            }
        }


        public void textBoxesChanged()
        {
            int index = framesListBox.selectedIndex;
            if (index >= 0 && index < curAnimateable.CurAnimation.numberFrames)
            {
                curAnimateable.CurAnimation.frames[index].assetName = textFieldFrameAsset.GetText();
                curAnimateable.CurAnimation.frames[index].frameName = textFieldframeName.GetText();
                string delay = textFieldFrameDelay.GetText();
                if (delay == "") delay = "1";
                curAnimateable.CurAnimation.frames[index].delay = Convert.ToInt32(delay);
                recomputeListBox();
            }
        }

        public void listBoxFrames_SelectedIndexChanged()
        {
            int ind = framesListBox.selectedIndex;
            controlPoints.Clear();
            if (ind != -1 && ind < curAnimateable.CurAnimation.frames.Count)
            {
                textFieldframeName.SetText(curAnimateable.CurAnimation.frames[ind].frameName);
                textFieldFrameAsset.SetText(curAnimateable.CurAnimation.frames[ind].assetName);
                textFieldFrameDelay.SetText(curAnimateable.CurAnimation.frames[ind].delay.ToString());
                curAnimateable.GotoAndStop(ind);
                try
                {
                    controlPoints = CpToCpUi(curAnimateable.CurAnimation.frames[curAnimateable.CurrentFrame].controlPoints);
                }
                catch (Exception)
                {
                  
                }
            }

            if (ind == -1)
            {
                textFieldFrameAsset.SetText("");
                textFieldframeName.SetText("");
                textFieldFrameDelay.SetText("");
            }
        }

        public void addFrame()
        {
            curAnimateable.CurAnimation.AddFrame("error", "");
            recomputeListBox();
        }

        public void delFrame()
        {
            int index = framesListBox.selectedIndex;
            if (index < curAnimateable.CurAnimation.numberFrames && index >= 0)
            {
                curAnimateable.CurAnimation.frames.RemoveAt(index);
                recomputeListBox();
            }
        }

        public void openAnimation()
        {
            string filename = FormManager.ShowFileOpenDialog(true);
            filename = filename.Replace(".anm", "");

            if (AssetManager.Animations.ContainsKey(filename))
            {
                controlPoints.Clear();
                curAnimateable.SetAnimation(AssetManager.GetAnimation(filename));
                curAnimateable.Stop();
                curAnimateable.StopLooping();
                recomputeListBox();
            }
        }

        public override void destroy()
        {
            openAnimationButton.active = false;
            saveAnimationButton.active = false;
            textFieldFrameAsset.active = false;
            textFieldframeName.active = false;
            newAnimationButton.active = false;
            delFrameButton.active = false;
            backButton.active = false;
            framesListBox.active = false;
            moveDownButton.active = false;
            moveUpButton.active = false;
            addFrameButton.active = false;
            stopLoopingButton.active = false;
            startLoopingButton.active = false;
            textFieldEndLooping.active = false;
            textFieldStartLooping.active = false;
            textFieldFrameDelay.active = false;
            textFieldFrameDelay = null;
            textFieldEndLooping = null;
            textFieldStartLooping = null;
            stopLoopingButton = null;
            startLoopingButton = null;
            openAnimationButton = null;
            saveAnimationButton = null;
            textFieldFrameAsset = null;
            textFieldframeName = null;
            newAnimationButton = null;
            framesListBox = null;
            moveDownButton = null;
            moveUpButton = null;
            addFrameButton = null;
            delFrameButton = null;
            backButton = null;
            controlPoints.Clear();

            base.destroy();
        }

        public void recomputeListBox()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < curAnimateable.CurAnimation.numberFrames; i++)
            {
                AnimationFrame a = curAnimateable.CurAnimation.frames[i];
                names.Add(a.assetName + ((a.frameName != "") ? " | " + a.frameName : ""));
            }
            framesListBox.dataSource = names;
            framesListBox.selectedIndex = -1;
        }

        public override void Update(GameTime time)
        {
            newAnimationButton.Update(time);
            openAnimationButton.Update(time);
            saveAnimationButton.Update(time);
            framesListBox.Update(time);
            addFrameButton.Update(time);
            delFrameButton.Update(time);
            textFieldFrameAsset.Update(time);
            textFieldframeName.Update(time);
            moveDownButton.Update(time);
            moveUpButton.Update(time);
            curAnimateable.AnimationUpdate();
            backButton.Update(time);
            textFieldStartLooping.Update(time);
            textFieldEndLooping.Update(time);
            startLoopingButton.Update(time);
            stopLoopingButton.Update(time);
            textFieldFrameDelay.Update(time);
            foreach (AnimationControlPointUI cp in controlPoints) { cp.Update(time); }
            if (framesListBox.selectedIndex >= 0 && framesListBox.selectedIndex < curAnimateable.CurAnimation.numberFrames)
                curAnimateable.CurAnimation.frames[framesListBox.selectedIndex].controlPoints = CpUiToCp(controlPoints);
            base.Update(time);
        }

        public override void Draw(SpriteBatch sb)
        {
            DrawManager.Draw_Box(new Vector2(2, 2), new Vector2(Main.graphics.PreferredBackBufferWidth - 2, 55), Color.Black, sb, 0f, 200);
            DrawManager.Draw_Outline(new Vector2(1, 1), new Vector2(Main.graphics.PreferredBackBufferWidth - 1, 56), Color.White, sb, 200);

            DrawManager.Draw_Box(new Vector2(Main.graphics.PreferredBackBufferWidth - 240, 60), new Vector2(Main.graphics.PreferredBackBufferWidth - 2, Main.graphics.PreferredBackBufferHeight - 2), Color.Black, sb, 0f, 200);
            DrawManager.Draw_Outline(new Vector2(Main.graphics.PreferredBackBufferWidth - 240, 60), new Vector2(Main.graphics.PreferredBackBufferWidth - 2, Main.graphics.PreferredBackBufferHeight - 2), Color.White, sb, 200);


            newAnimationButton.Draw(sb);
            openAnimationButton.Draw(sb);
            saveAnimationButton.Draw(sb);

            framesListBox.Draw(sb);

            addFrameButton.Draw(sb);
            delFrameButton.Draw(sb);
            moveDownButton.Draw(sb);
            moveUpButton.Draw(sb);
            backButton.Draw(sb);
            startLoopingButton.Draw(sb);
            stopLoopingButton.Draw(sb);

            textFieldFrameAsset.Draw(sb);
            textFieldframeName.Draw(sb);
            textFieldEndLooping.Draw(sb);
            textFieldStartLooping.Draw(sb);
            textFieldFrameDelay.Draw(sb);

            Texture2D frame = curAnimateable.GetCurrentFrame();
            sb.Draw(frame, Main.Center, null, Color.White, 0f, new Vector2(frame.Width / 2, frame.Height / 2), imgScale, SpriteEffects.None, 0f);

            if (!curAnimateable.Playing)
            {
                foreach (AnimationControlPointUI cp in controlPoints)
                {
                    cp.Draw(sb);
                }
            }

            base.Draw(sb);
        }

        public void newAnimation()
        {
            curAnimateable.CurAnimation = new Animation();
            recomputeListBox();
        }

        public void saveAnimation()
        {

            string savePath = FormManager.ShowFileSaveDialog("animation", ".anm");
            string save = JSONManager.Serialize(curAnimateable.CurAnimation);
            FileWriterManager.WriteFile(savePath, save.TrimEnd('\n'));
            ConsoleManager.ConsoleInput("reset_textures", true);
        }
    }
}
