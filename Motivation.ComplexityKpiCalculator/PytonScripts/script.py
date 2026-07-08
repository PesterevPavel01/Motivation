import re
import os
import shutil
import ezdxf
from ezdxf.math import BoundingBox


def process_nesting_dxf(input_path, output_path):
    doc = ezdxf.readfile(input_path)
    msp = doc.modelspace()
    layer_colors = {
        'R2': 1,
        'R3': 2,
        'R90': 3,
        'R6': 6,
        'R+F': 5,
        'R1': 3,
        'R4': 4,
    }
    texts = []
    for entity in msp.query('TEXT MTEXT'):
        texts.append((entity.dxf.text.strip(), entity.dxf.insert))

    counts = {}
    for poly in msp.query('POLYLINE LWPOLYLINE'):
        if poly.dxf.layer != '0':
            continue
        if poly.dxftype() == 'POLYLINE':
            points = [v.dxf.location for v in poly.vertices]
        else:
            points = poly.vertices()
        bbox = BoundingBox(points)
        found_text = None
        for txt, pos in texts:
            if (
                bbox.extmin.x <= pos.x <= bbox.extmax.x
                and bbox.extmin.y <= pos.y <= bbox.extmax.y
            ):
                found_text = txt
                break
        if found_text:
            match = re.search(r'(?:^|_)((?:R|R\s)[^_\*]*)', found_text)
            if match:
                raw = match.group(1).strip()
                layer_name = (
                    re.sub(r'(?:мм|\xef\xbf\xbd|\s)+$', '', raw)
                    .replace(' ', '')
                    .replace('-', '')
                )
                color = layer_colors.get(layer_name, 7)
                if layer_name not in doc.layers:
                    doc.layers.add(name=layer_name, color=color)
                poly.dxf.layer = layer_name
                poly.dxf.color = color
                counts[layer_name] = counts.get(layer_name, 0) + 1

    doc.saveas(output_path)
    print(f'✅ {os.path.basename(input_path)} → готово, статистика: {counts}')
    return counts


def clean_folder(folder_path):
    """Очищает папку result_files перед началом работы"""
    if os.path.exists(folder_path):
        shutil.rmtree(folder_path)
        print(f'🧹  Папка "{folder_path}" очищена')
    os.makedirs(folder_path, exist_ok=True)
    print(f'📁  Папка "{folder_path}" создана/готова')


def process_all_files():
    # Используем сетевые пути
    source_dir = r'\\DESKTOP-4L56RF4\scripts\source_files'
    result_dir = r'\\DESKTOP-4L56RF4\scripts\result_files'
    
    # 1. Очищаем папку result_files
    clean_folder(result_dir)
    
    # 2. Проверяем, существует ли папка source_files
    if not os.path.exists(source_dir):
        print(f'❌ Папка "{source_dir}" не найдена!')
        print('Проверьте доступность сетевой папки и права доступа.')
        return
    
    # 3. Собираем все DXF файлы
    dxf_files = [f for f in os.listdir(source_dir) if f.lower().endswith('.dxf')]
    
    if not dxf_files:
        print(f'❌ В папке "{source_dir}" нет DXF файлов!')
        return
    
    print(f'🔍 Найдено {len(dxf_files)} DXF файлов: {", ".join(dxf_files)}')
    print('=' * 50)
    
    # 4. Обрабатываем каждый файл
    total_stats = {}
    for filename in dxf_files:
        input_path = os.path.join(source_dir, filename)
        output_path = os.path.join(result_dir, filename)
        
        try:
            stats = process_nesting_dxf(input_path, output_path)
            total_stats[filename] = stats
        except Exception as e:
            print(f'❌ Ошибка при обработке "{filename}": {e}')
    
    # 5. Итоговый отчет
    print('=' * 50)
    print('📊  ОБЩАЯ СТАТИСТИКА:')
    for filename, stats in total_stats.items():
        print(f'   {filename}: {stats}')
    print('✅ Все файлы обработаны!')


if __name__ == '__main__':
    process_all_files()