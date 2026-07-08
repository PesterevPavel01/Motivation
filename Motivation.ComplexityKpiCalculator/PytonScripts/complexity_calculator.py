import sys
import os
import json
from datetime import datetime
from typing import Dict, Any

script_dir = os.path.dirname(os.path.abspath(__file__))
log_path = os.path.join(script_dir, "python_debug.log")

log_file = open(log_path, "a", encoding="utf-8")

def log_debug(msg):
    timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S.%f")[:-3]
    log_file.write(f"[{timestamp}] {msg}\n")
    log_file.flush()
    print(f"[{timestamp}] {msg}", file=sys.stderr, flush=True)

def format_data(data: Dict[str, Any]) -> str:
    try:
        return json.dumps(data, indent=2, ensure_ascii=False, default=str)
    except:
        return str(data)

log_debug("=== Python script started ===")

class ComplexityCalculator:
    def __init__(self):
        self.name = "KpiCalculator"
        self.version = "1.0.0"

    def calculate_complexity(self, data: Dict[str, Any]) -> float:
        period_from = datetime.strptime(data.get('period_from', ''), '%Y-%m-%d')
        period_to = datetime.strptime(data.get('period_to', ''), '%Y-%m-%d')
        days = (period_to - period_from).days + 1
        
        result = days * 1000
        
        return float(result)

def calculate_complexity_kpi(data: Dict[str, Any]) -> float:
    log_debug("INPUT DATA:")
    log_debug(format_data(data))
    
    calculator = ComplexityCalculator()
    result = calculator.calculate_complexity(data)
    
    log_debug(f"FINAL RESULT: {result}")
    
    return result