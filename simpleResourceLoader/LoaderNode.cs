//simple loader to get some resources from disc to mem and keep track of loading process

using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

class LoaderNode : Node
{
    #region signals to connect

    [Signal]
    delegate void loadingPercentChanged(float newValue);

    [Signal]
    delegate void loadingDoneResources(List<Resource> loadedResources);

    [Signal]
    delegate void loadingDoneScenes(List<PackedScene> loadedScenes);

    #endregion

    #region editor variables

    private float _minValue = 0;
    /// <summary>
    /// "nothing loaded yet" marker (cant be less than zero)
    /// </summary>
    [Export(PropertyHint.Range, "0,99,or_greater")]
    public float minValue
    {
        get { return _minValue; }
        set
        {
            _minValue = value;
            if (_minValue < 0) _minValue = 0;
        }
    }

    private float _maxValue = 1;
    /// <summary>
    /// "all loaded" marked (cant be less than one)
    /// </summary>
    [Export(PropertyHint.Range, "1,100,or_greater")]
    public float maxValue
    {
        get { return _maxValue; }
        set
        {
            _maxValue = value;
            if (_maxValue < 1) _maxValue = 1;
        }
    }

    /// <summary>
    /// where loader should look for listed resources
    /// </summary>
    [Export(PropertyHint.Dir)]
    public string loadRootDir = "res://";

    #endregion

    #region other variables

    private float _loadingPercentValue;
    /// <summary>
    /// current loading percent value (will be minValue at loading start and maxValue at loading done)
    /// </summary>
    public float loadingPercentValue
    {
        get { return _loadingPercentValue; }
        set
        {
            _loadingPercentValue = value;
            EmitSignal(nameof(loadingPercentChanged), _loadingPercentValue);
        }
    }

    private bool anyResourcesLoading = false;

    #endregion

    #region start loading funcs

    public bool StartLoadingResources(List<string> resourcesPaths, string subfolder = "")
    {
        if (anyResourcesLoading)
        {
            return false;
        }

        LoadTask(nameof(loadingDoneResources), resourcesPaths, subfolder).Start();

        return true;
    }

    public bool StartLoadingScenes(List<string> resourcesPaths, string subfolder = "")
    {
        if (anyResourcesLoading)
        {
            return false;
        }
        
        LoadTask(nameof(loadingDoneScenes), resourcesPaths, subfolder).Start();

        return true;
    }

    #endregion

    private Task LoadTask(string signal, List<string> resourcesPaths, string subfolder)
    {
        return new Task(() =>
        {
            anyResourcesLoading = true; //we are busy

            _loadingPercentValue = minValue; //setting our percent indicator to start position

            float loadingPercentStep = maxValue / resourcesPaths.Count; //calculating indicators step

            List<Resource> loadedResources = new List<Resource>();

            Resource loadedOne;

            foreach (string path in resourcesPaths) //lets iterrate our paths
            {
                string fullPath = loadRootDir + subfolder + "/" + path; //make fullpath out of current

                if (!ResourceLoader.Exists(fullPath)) //check for existence
                {
                    GD.PushWarning("resource [" + fullPath + "] not found, skipping.");

                    loadingPercentValue += loadingPercentStep;
                    continue;
                }

                loadedOne = GD.Load(path); //try to load

                if (loadedOne == null) //if failed
                {
                    GD.PushWarning("resource [" + fullPath + "] not recognized, skipping.");
                    loadingPercentValue += loadingPercentStep;

                    continue;
                }

                loadedResources.Add(loadedOne); //add to list of loaded if sucessed

                loadingPercentValue += loadingPercentStep;
            }

            anyResourcesLoading = false; //when done we are not that busy (:

            if (loadedResources.Count == 0) //if nothing was added to our loaded list
            {
                GD.PushWarning("nothing was loaded.");                

                EmitSignal(signal, null);
                
                return;
            }

            if (signal == nameof(loadingDoneScenes)) //if we was loading scenes -> safely cast loaded resources to them
            {
                EmitSignal(signal, loadedResources.OfType<PackedScene>().ToList());
            }
            else
            {
                EmitSignal(signal, loadedResources);
            }
        });        
    }
}
