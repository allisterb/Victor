import io
import json

from snips_nlu.cli import training
from snips_nlu.cli import generate_dataset
from snips_nlu import SnipsNLUEngine
from snips_nlu.default_configs import CONFIG_EN


generate_dataset.generate_dataset("en", "..\Victor.NLU.Snips\Datasets\Hello.yaml")
#training.train("..\Victor.NLU.Snips\Datasets\Hello.yaml", output_path="..\Victor.NLU.Snips\Datasets\Hello.json")

engine = SnipsNLUEngine(config=CONFIG_EN)


with io.open("..\Victor.NLU.Snips\Datasets\Hello.json") as f:
    dataset = json.load(f)

engine.fit(dataset)
