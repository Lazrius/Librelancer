using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using LibreLancer;
using LibreLancer.ContentEdit;
using LibreLancer.GameData;
using LibreLancer.ImUI;
using LibreLancer.Render.Cameras;
using LibreLancer.Sounds;

namespace LancerEdit;

public class GameDataContext : IDisposable
{
    public GameDataManager GameData;
    public GameResourceManager Resources;
    public SoundManager Sounds;
    public FontManager Fonts;

    private MainWindow win;

    public EditableInfocardManager Infocards => (EditableInfocardManager)GameData.Ini.Infocards;

    public string Folder;

    public void Load(MainWindow win, string folder, Action onComplete)
    {
        Folder = folder;
        Resources = new GameResourceManager(win);
        this.win = win;
        Task.Run(() =>
        {
            GameData = new GameDataManager(folder, Resources);
            GameData.LoadData(win);
            //Replace infocard manager with editable version
            GameData.Ini.Infocards = new EditableInfocardManager(GameData.Ini.Infocards.Dlls);
            FLLog.Info("Game", "Finished loading game data");
            win.QueueUIThread(() =>
            {
                Sounds = new SoundManager(GameData, win.Audio, win);
                Fonts = new FontManager();
                Fonts.LoadFontsFromGameData(GameData);
                onComplete();
            });
        });
    }


    private int renderCounter = 0;
    

    public bool RenderAllArchetypePreviews()
    {
        //Delay Processing so messages can show
        ImGuiHelper.AnimatingElement();
        if (renderCounter < 5) {
            renderCounter++;
            return false;
        }
        //Do things
        using var rm = new GameResourceManager(Resources);
        using var renderer = new ArchetypePreviews(win, rm);
        int j = 0;
        foreach (var a in GameData.Archetypes)
        {
            j += GetArchetypePreview(a, renderer);
            if(rm.EstimatedTextureMemory > 128 * 1024 * 1024)
                rm.ClearTextures();
        }
        return true;
    }

    private Dictionary<string, (Texture2D, int)> renderedArchetypes = new Dictionary<string, (Texture2D, int)>();
    
    public int GetArchetypePreview(Archetype archetype, ArchetypePreviews renderer = null)
    {
        if (renderedArchetypes.TryGetValue(archetype.Nickname, out var arch))
            return renderer != null ? 0 : arch.Item2;
        Texture2D tx;
        if(renderer != null) 
            tx = renderer.RenderPreview(archetype, 128, 128);
        else
        {
            using var r2 = new ArchetypePreviews(win, Resources);
            tx = r2.RenderPreview(archetype, 128, 128);
        }
        arch = (tx, ImGuiHelper.RegisterTexture(tx));
        renderedArchetypes[archetype.Nickname] = arch;
        return renderer != null ? 1 : arch.Item2;
    }
    
    public void Dispose()
    {
        Sounds.Dispose();
        Resources.Dispose();
        foreach (var ax in renderedArchetypes)
        {
            ImGuiHelper.DeregisterTexture(ax.Value.Item1);
            ax.Value.Item1.Dispose();
        }
    }
}