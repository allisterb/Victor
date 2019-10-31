import sys
import io
import os
from io import TextIOWrapper, BytesIO
from contextlib import redirect_stdout
import json

from snips_nlu.cli import training
from snips_nlu.cli import generate_dataset
from snips_nlu import SnipsNLUEngine
from snips_nlu.default_configs import CONFIG_EN

engine = SnipsNLUEngine(config=CONFIG_EN)

def train_model(_input, name):
    input = os.path.join("..", "Victor.NLU.Snips", "Datasets", _input)
    output = input.replace(".yaml", ".json")
    with TextIOWrapper(io.FileIO(output, "wb"), sys.stdout.encoding) as buf, redirect_stdout(buf):
        generate_dataset.generate_dataset("en", input)
        buf.flush()
        buf.close()
    
    with io.open(output) as f:
        dataset = json.load(f)
    engine.fit(dataset)
    engine.persist(os.path.join(os.path.dirname(input), "..", "Engines", name))

    