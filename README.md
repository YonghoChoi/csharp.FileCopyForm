# csharp.FileCopyForm

## 파일 복사 모듈

### 목표

* 해당 프로그램을 사용하기 위한 목적이 아닌 파일 복사가 필요한 프로젝트에서 쉽게 모듈을 가져다 사용할 수 있도록 하는 것이 목표입니다.

### 환경

* Skills : C#
* Tools : Visual Studio 2013

### 진행 상황

* 현재 기본적인 파일 복사만 가능합니다. (파일 복사, 폴더 복사)
* progress bar에 대한 보완이 필요합니다.
  * size 큰 파일의 경우 overflow 발생
    * progress bar의 범위 값에 파일 총 사이즈를 대입하여 사용했는데 타입 불일치(progress bar = int, totalsize = long)
* 복사 속도에 대한 성능 향상이 필요합니다.
