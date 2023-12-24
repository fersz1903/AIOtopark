function saveToJsonFile() {
    // JSON verisini bir dosyaya yaz
    var jsonContent = JSON.stringify(posList, null, 2);

    // Blob oluştur
    var blob = new Blob([jsonContent], { type: 'application/json' });

    // Blob URL'sini oluştur
    var blobUrl = URL.createObjectURL(blob);

    // Dosyayı indirme bağlantısını oluştur
    var downloadLink = document.createElement('a');
    downloadLink.href = blobUrl;
    downloadLink.download = 'posList.json'; // İndirilen dosyanın adı

    // Dosyayı indir
    downloadLink.click();

    // Blob URL'sini serbest bırak (belleği temizle)
    URL.revokeObjectURL(blobUrl);
}

//function saveAndClose() {
//    // Kullanıcının yaptığı işlemler sonucu elde edilen JSON içeriği
//    var jsonData = { key: 'value' }; // Örnek JSON

//    // Ana sayfadaki fonksiyonu çağırarak JSON içeriğini iletiyoruz
//    window.opener.handleSave(jsonData);

//    // Pencereyi kapat
//    window.close();
//}


var posList = [];
var height = 30;
var width = 65;

function readImage(input) {
    var file = input.files[0];

    if (file) {
        var reader = new FileReader();

        reader.onload = function (e) {
            var image = new Image();
            image.src = e.target.result;
            image.onload = function () {
                createCanvas(image);
                $(canvasContainer).on('click', selectPixel);
            };
        };

        reader.readAsDataURL(file);
    } else {
        console.log('Image not selected.');
    }
}
function createCanvas(image) {
    var canvasContainer = $('#canvasContainer')[0];

    // Orijinal resmi tutacak canvas
    var originalCanvas = document.createElement('canvas');
    originalCanvas.width = image.width;
    originalCanvas.height = image.height;
    originalCanvas.getContext('2d').drawImage(image, 0, 0);
    originalCanvas.id = "canvas1";

    // Dikdörtgenleri tutacak canvas
    var rectangleCanvas = document.createElement('canvas');
    rectangleCanvas.width = image.width;
    rectangleCanvas.height = image.height;
    rectangleCanvas.id = "canvas2";

    // Container'a canvas'ları ekleyin
    canvasContainer.innerHTML = '';
    canvasContainer.appendChild(originalCanvas);
    canvasContainer.appendChild(rectangleCanvas);
}

function selectPixel(e) {
    var x = e.offsetX;
    var y = e.offsetY;

    // Dikdörtgenleri tutan canvas'ı al
    var rectangleCanvas = $('#canvasContainer canvas')[1];
    var ctx = rectangleCanvas.getContext('2d');

    // Sol tık ile yeni dikdörtgeni çiz
    if (e.button === 0) {
        posList.push([x, y]);
        drawRectangles(ctx);
    } else if (e.button === 2) {
        // Sağ tık ile tıklanan dikdörtgeni sil. resim olarak algılandığı için çalışmıyor
        var indexToRemove = findClickedRectangle(x, y);
        if (indexToRemove !== -1) {
            posList.splice(indexToRemove, 1);
            drawRectangles(ctx);
        }
    }
}


var mouseX, mouseY;

// Mouse hareketi dinleme
document.addEventListener('mousemove', function (e) {
    mouseX = e.clientX;
    mouseY = e.clientY;

    // Koordinatları ekrana yazdırma
    document.getElementById('mouse-coordinates').innerText = 'Mouse Coordinates: (' + mouseX + ', ' + mouseY + ')';
});

document.addEventListener('keydown', function (e) {
    // Check the pressed key code
    switch (e.key) {
        case 'd':
            // son dikdörtgeni sil
            posList.pop();
            var rectangleCanvas = $('#canvasContainer canvas')[1];
            var ctx = rectangleCanvas.getContext('2d');

            drawRectangles(ctx);
            break;

        default:
            break;
    }
});


function drawRectangles(ctx) {
    ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);

    // Dikdörtgenleri çiz
    posList.forEach(function (pos) {
        ctx.strokeStyle = 'blue';
        ctx.lineWidth = 1;
        ctx.strokeRect(pos[0], pos[1], width, height);
    });
}

function findClickedRectangle(x, y) {
    for (var i = 0; i < posList.length; i++) {
        var pos = posList[i];
        if (x >= pos[0] && x <= pos[0] + width && y >= pos[1] && y <= pos[1] + height) {
            return i;
        }
    }
    return -1;
}

function clearSelection() {
    posList = [];
    var rectangleCanvas = $('#canvasContainer canvas')[1];
    var ctx = rectangleCanvas.getContext('2d');
    drawRectangles(ctx);
}

$('#photoInput').change(function () {
    readImage(this);
});

