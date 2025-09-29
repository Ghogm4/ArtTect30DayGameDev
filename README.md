A game project from ArtTech 30-day game development.
  
# Git LFS 配置
首先通过 https://git-lfs.com/ 下载git lfs插件  

首次clone仓库时，在当前仓库的根目录的终端中运行
```
git lfs install
```
之后正常进行git pull与push即可  

检查当前已追踪的文件格式，可以使用  
```
git lfs track
```


如果在工程中引入了新的大型文件的格式，例如工程中新增了.png文件，可以使用
```
git lfs track "*.png"
```
来将所有的png文件加入到追踪列表  
普通的脚本文件等不大的文件不需要被追踪
