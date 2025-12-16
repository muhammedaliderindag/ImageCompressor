# 📷 ImageCompressor

🔗 **Canlı Demo:** [Projeyi Görüntüle](https://imagestudio.mazosfer.com/)

**ImageCompressor**, görüntüleri web arayüzü üzerinden yükleyip optimize etmenizi (sıkıştırmanızı) sağlayan, modern teknolojilerle geliştirilmiş açık kaynaklı bir web uygulamasıdır.

Bu proje **.NET** altyapısı üzerine kurulmuş olup, kolay dağıtım için **Docker** desteği entegrasyonu içermektedir.

## 🚀 Özellikler

* **Web Tabanlı Arayüz:** Kullanıcı dostu arayüz ile kolayca görsel yükleme ve işlem yapma imkanı.
* **Görüntü Sıkıştırma:** Yüksek boyutlu görselleri kalite kaybını minimize ederek optimize eder.
* **Docker Desteği:** `docker-compose` ile tek komutla tüm ortamı ayağa kaldırabilirsiniz.
* **Modern Mimari:** C# ve .NET teknolojileri ile geliştirilmiş modüler yapı.

## 🛠️ Teknolojiler

* **Backend & Frontend:** .NET (C#), HTML, CSS
* **Containerization:** Docker, Docker Compose
* **Monitoring:** Prometheus

## ⚙️ Kurulum ve Çalıştırma

Projeyi yerel makinenizde çalıştırmak için aşağıdaki adımları izleyebilirsiniz.

### Ön Koşullar

* [Docker Desktop](https://www.docker.com/products/docker-desktop) (veya Docker Engine + Compose)
* [Git](https://git-scm.com/)

### Adım Adım Kurulum

1.  **Projeyi Klonlayın:**
    ```bash
    git clone [https://github.com/muhammedaliderindag/ImageCompressor.git](https://github.com/muhammedaliderindag/ImageCompressor.git)
    cd ImageCompressor
    ```

2.  **Docker ile Başlatın:**
    Proje dizinindeyken aşağıdaki komutu çalıştırarak uygulamayı ve gerekli servisleri başlatın:
    ```bash
    docker-compose up -d --build
    ```

3.  **Uygulamaya Erişin:**
    Kurulum tamamlandıktan sonra tarayıcınızdan aşağıdaki adreslere gidebilirsiniz:
    * **Web Uygulaması:** `http://localhost:8080` (Veya `docker-compose.yml` dosyasında yapılandırılan port)

## 📂 Proje Yapısı

* `ImageCompressor.Web/`: Web uygulamasının kaynak kodlarını barındırır.
* `docker-compose.yml`: Servislerin (App, Prometheus vb.) orkestrasyonunu sağlar.

## 📝 Lisans

Bu proje [MIT Lisansı](LICENSE) altında sunulmaktadır.

---

**Geliştirici:** [Muhammed Ali Derindağ](https://github.com/muhammedaliderindag)
