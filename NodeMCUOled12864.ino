#include <Arduino.h>
#include <U8g2lib.h>
#ifdef U8X8_HAVE_HW_SPI
#include <SPI.h>
#endif
#ifdef U8X8_HAVE_HW_I2C
#include <Wire.h>
#endif

U8G2_SSD1306_128X64_NONAME_F_HW_I2C u8g2(U8G2_R0, /* reset=*/ U8X8_PIN_NONE);

unsigned char col[1024] = {};
unsigned int n = 0;

void setup(void) {
  u8g2.begin();
  u8g2.clearBuffer();

  Serial.begin(1000000);  
  while (Serial.available() > 0)
    Serial.read();
  printStr("Running");
}

void loop(void) {
  if (Serial.available() > 0 && n < 1024) {
    while (Serial.available() > 0) {
      col[n++] = Serial.read();
    }
    if (n < 1024) {
      return;
    }

    u8g2.clearBuffer();
    u8g2.drawXBM(0, 0, 128, 64, col);
    u8g2.sendBuffer();
    n = 0;
  }
}

void printStr(char* str) {
  u8g2.clearBuffer();          // clear the internal memory
  u8g2.setFont(u8g2_font_ncenB08_tr); // choose a suitable font
  u8g2.drawStr(10, 10, str); // write something to the internal memory
  u8g2.sendBuffer();
}
