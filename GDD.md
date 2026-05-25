# **City Checkpoint Driver - Game Design Document**

## **Game Overview**

### **Spesifikasi Game**

- **Judul Game:** City Checkpoint Driver
- **Genre:** Arcade Driving, Racing Casual
- **Tema:** Berkendara di kota, checkpoint challenge, free drive
- **Target Pengguna:** Usia 10 tahun ke atas, pemain casual, pemain yang suka game mobil sederhana
- **Platform:** PC
- **Waktu Pengembangan:** MVP playable feature pass selesai

### **Tim Pengembang:**

**Game Designer:**

1. -

**Artist:**

1. -

**Game Programmer:**

1. -

**Sound Engineer:**

1. -

**Produser/Manajer Produk:**

1. -

---

## **Background**

**City Checkpoint Driver** dibuat sebagai game mengemudi 3D sederhana yang berfokus pada tantangan melewati checkpoint di area kota. Game ini dikembangkan dari prototype awal mobil yang sudah bisa dikendarai, kemudian diperluas menjadi MVP yang memiliki main menu, level, free drive, checkpoint system, HUD, audio, save progress, dan sistem unlock level.

Game ini sengaja dibuat dengan scope kecil agar bisa dipublish lebih cepat. Konten awal hanya berisi 2 level playable dan mode Free Drive. Level berikutnya ditampilkan sebagai **Coming Soon** agar game tetap terasa siap dikembangkan lebih lanjut.

---

## **Pitch**

**City Checkpoint Driver** adalah game arcade driving 3D sederhana di mana pemain mengendarai mobil melewati checkpoint berurutan sebelum waktu habis, atau berkendara bebas di mode Free Drive tanpa misi dan timer.

---

## **Core Experience**

Pemain harus merasakan pengalaman mengemudi yang mudah dipahami, responsif, dan langsung menyenangkan. Tujuan utama pemain adalah menemukan checkpoint berikutnya, mengatur kecepatan, berbelok dengan baik, dan menyelesaikan level secepat mungkin.

Untuk pemain yang ingin santai, mode **Free Drive** memberikan pengalaman eksplorasi kota tanpa tekanan misi, timer, atau fail condition.

---

## **Game Core Direction**

Arah utama game adalah menghadirkan pengalaman **arcade driving simple** yang mudah dimainkan dan cepat dipublish. Fokus utama bukan simulasi realistis, melainkan:

- kontrol mobil stabil dan responsif
- checkpoint mudah terlihat
- level pendek dan jelas
- sistem unlock sederhana
- mode bebas untuk latihan dan eksplorasi

Game tidak menambahkan sistem kompleks seperti traffic AI, police chase, upgrade mobil, garage, delivery, atau multiplayer pada MVP pertama.

---

## **Game Flow Summary**

Pemain memulai dari main menu. Dari menu, pemain dapat memilih **Level Select** atau **Free Drive**. Pada mode checkpoint, pemain memilih level yang tersedia, mobil akan spawn di titik start, timer berjalan, dan checkpoint pertama aktif. Pemain harus melewati checkpoint secara berurutan sampai semua checkpoint selesai. Setelah level selesai, result screen menampilkan waktu dan stars.

Jika pemain memilih **Free Drive**, mobil spawn di kota tanpa timer, tanpa checkpoint, dan tanpa fail condition. Pemain bisa berkendara bebas, pause, restart ke spawn, atau kembali ke main menu.

---

## **Look and Feel**

Game menggunakan visual 3D low-poly city dari city pack. Tampilan dibuat simple dan jelas agar pemain mudah membaca jalan, checkpoint, dan arah tujuan.

Checkpoint dibuat dengan visual:

- warna merah
- transparansi 50%
- bentuk tinggi dan sempit
- ring menempel di tanah
- vertical beam dari tanah
- marker di bagian atas
- point light merah
- sedikit animasi pulse/rotation

UI dibuat sederhana dengan teks besar, tombol jelas, HUD timer, checkpoint count, speedometer, direction arrow, distance text, dan minimap sederhana.

---

## **Core Loop (Bisa dibuat Diagram)**

1. **Pilih Mode:** Pemain memilih Checkpoint Level atau Free Drive dari main menu.
2. **Berkendara:** Pemain mengontrol mobil menggunakan gas, steer, brake, dan handbrake.
3. **Ikuti Tujuan:** Pada checkpoint mode, pemain mengikuti arrow, distance text, dan minimap menuju checkpoint aktif.
4. **Selesaikan Level:** Pemain melewati semua checkpoint sebelum timer habis.
5. **Dapat Progress:** Game menyimpan best time, stars, dan unlock level berikutnya.
6. **Retry atau Lanjut:** Pemain bisa retry untuk waktu lebih baik, lanjut ke level berikutnya, atau kembali ke main menu.

---

## **Mekanik Inti Game**

Mekanik inti dari **City Checkpoint Driver** berfokus pada mengemudi dan menyelesaikan checkpoint berurutan. Semua sistem dibuat sederhana agar mudah dipahami pemain dan mudah dikembangkan untuk level tambahan.

### **Highlight Mekanik 1: Car Controller**

- **Forward/Reverse:** Pemain bisa maju dan mundur menggunakan input keyboard.
- **Steering:** Mobil bisa berbelok kiri/kanan dengan steering yang stabil.
- **Brake:** Pemain dapat mengerem menggunakan tombol brake.
- **Handbrake:** Pemain dapat memakai handbrake untuk membantu belok tajam.
- **Wheel Visual Sync:** Visual roda mengikuti WheelCollider dan sudah diperbaiki agar rotasi roda tidak salah.
- **Stable Driving:** Rigidbody, collider, center of mass, downforce, dan lateral grip diatur agar mobil tidak mudah terguling.

### **Highlight Mekanik 2: Checkpoint Race**

- Checkpoint aktif satu per satu.
- Pemain harus melewati checkpoint sesuai urutan.
- Checkpoint berikutnya aktif setelah checkpoint saat ini dilewati.
- Timer berjalan selama level.
- Level complete jika semua checkpoint selesai sebelum timer habis.
- Level failed jika timer habis.
- Checkpoint memiliki sound feedback menggunakan `checkpoint.mp3`.

### **Highlight Mekanik 3: Free Drive**

- Mode bebas tanpa misi.
- Tidak ada timer.
- Tidak ada checkpoint.
- Tidak ada win/fail condition.
- Cocok untuk latihan mengemudi1: Game State dan Game Mode** dan eksplorasi kota.
- Restart mengembalikan mobil ke spawn Free Drive.

### **Highlight Mekanik 4: Direction Helper dan Mini Map**

- Direction arrow menunjukkan arah checkpoint aktif.
- Distance text menampilkan jarak menuju checkpoint.
- Mini map sederhana menampilkan posisi player dan checkpoint.
- Tujuan sistem ini adalah mengurangi kebingungan pemain saat mencari checkpoint.

---

## **Sistem Utama**

### **Sistem 1: Sistem Level dan Checkpoint**

Pemain menyelesaikan level dengan melewati checkpoint secara berurutan sebelum waktu habis. Setiap checkpoint yang berhasil dilewati akan mengaktifkan checkpoint berikutnya. Level selesai jika semua checkpoint berhasil dilewati.

Pada versi MVP, game memiliki 2 level playable. Level berikutnya ditampilkan sebagai **Coming Soon** agar pemain tahu bahwa konten akan bertambah.

### **Sistem 2: Mode Bermain**

Game memiliki dua mode utama:

- **Checkpoint Level:** Mode utama dengan timer, checkpoint, objective, dan result screen.
- **Free Drive:** Mode bebas tanpa misi, tanpa timer, dan tanpa gagal. Mode ini digunakan untuk latihan mengemudi dan eksplorasi kota.

Kedua mode tetap menggunakan mobil, camera, dan kontrol yang sama agar pengalaman bermain konsisten.

### **Sistem 3: Sistem Progress dan Stars**

Progress pemain disimpan secara otomatis. Pemain dapat membuka Level 2 setelah menyelesaikan Level 1. Setiap level juga menyimpan waktu terbaik dan jumlah bintang terbaik.

Bintang diberikan berdasarkan waktu penyelesaian:

- **1 Star:** Level selesai.
- **2 Stars:** Level selesai dengan waktu lebih baik.
- **3 Stars:** Level selesai dengan waktu terbaik.

### **Sistem 4: Sistem UI dan Navigasi**

UI membantu pemain memahami kondisi permainan. Main menu digunakan untuk memilih mode bermain, level select digunakan untuk memilih level, HUD digunakan saat bermain, pause menu digunakan untuk berhenti sementara, dan result screen muncul setelah level selesai atau gagal.

Saat mode checkpoint, HUD menampilkan timer, jumlah checkpoint, speedometer, arah checkpoint, jarak checkpoint, dan minimap sederhana. Saat Free Drive, HUD dibuat lebih minimal agar pemain bisa fokus berkendara.

### **Sistem 5: Sistem Audio**

Audio digunakan untuk membuat game terasa lebih hidup. Game memiliki musik latar, suara mesin mobil, suara start mobil, suara tabrakan, klakson, dan suara ketika checkpoint berhasil dilewati.

Suara mesin mobil mengikuti kecepatan mobil agar berkendara terasa lebih responsif.

### **Sistem 6: Sistem Kamera**

Kamera mengikuti mobil dari belakang dengan gerakan halus. Kamera membantu pemain melihat jalan dan checkpoint berikutnya dengan jelas. Game menggunakan camera utama yang ada di hierarchy `_Scene/Camera`.

---

## **Level Design**

### **Level 1 - Tutorial Route**

- **Status:** Playable
- **Jumlah Checkpoint:** 4
- **Fokus:** Basic control
- **Tujuan:** Pemain memahami gas, rem, steer, dan cara melewati checkpoint.
- **Difficulty:** Mudah

### **Level 2 - City Corners**

- **Status:** Playable
- **Jumlah Checkpoint:** 6
- **Fokus:** Turning dan membaca rute
- **Tujuan:** Pemain mulai belajar mengatur kecepatan sebelum belokan.
- **Difficulty:** Lebih tinggi dari Level 1

### **Level 3+ - Coming Soon**

- **Status:** Belum playable
- **Fungsi:** Ditampilkan di Level Select sebagai konten yang akan datang.
- **Catatan:** Sistem sudah scalable untuk menambahkan level baru nanti.

---

## **Scoring dan Progression**

### **Stars**

- **1 Star:** Level selesai sebelum timer habis.
- **2 Stars:** Level selesai lebih cepat dari target medium.
- **3 Stars:** Level selesai lebih cepat dari target best.

### **Progression**

- Level 1 terbuka dari awal.
- Level 2 unlock setelah Level 1 selesai.
- Level 3+ disabled dan diberi label **Coming Soon**.

---

## **Controls**

### **Keyboard**

- **W / Arrow Up:** Gas
- **S / Arrow Down:** Reverse
- **A / D:** Steer kiri/kanan
- **Space:** Brake
- **Shift:** Handbrake
- **H:** Honk
- **Esc:** Pause / Resume

---

## **Scene Structure**

Scene utama sudah dipindah dan rename menjadi:

```text
Assets/Scenes/main.unity
```

Struktur hierarchy utama:

```text
_Scene
  Environment
  Gameplay
    PlayerCar
      Visuals
      Physics
      Scripts
    SpawnPoints
      FreeDriveSpawn
      Level01Spawn
      Level02Spawn
    Checkpoints
      Level01
        CP_01 ... CP_04
      Level02
        CP_01 ... CP_06
  Camera
    Main Camera
  Lighting
  UI
    GameCanvas
  Managers
    GameManager
    AudioManager
```

---

## **Production Ready Criteria**

MVP dianggap siap publish jika:

- Game bisa launch dari main menu.
- Free Drive bisa dimainkan.
- Level 1 bisa complete/fail.
- Level 2 bisa unlock dan dimainkan.
- Level 3+ tampil sebagai Coming Soon.
- Restart selalu kembali ke spawn.
- Checkpoint mudah ditemukan menggunakan visual, arrow, distance text, dan minimap.
- Save progress berjalan setelah restart game.
- Audio utama bekerja.
- Camera render dari `_Scene/Camera/Main Camera`.
- Tidak ada console error fatal.
- PC build berhasil dan bisa dimainkan dari fresh launch.

---

## **REFERENSI GAME**

1. **Crazy Taxi** — inspirasi arcade driving sederhana dan cepat.
2. **Trackmania** — inspirasi time-based driving challenge.
3. **Need for Speed checkpoint/time trial mode** — inspirasi checkpoint race casual.
