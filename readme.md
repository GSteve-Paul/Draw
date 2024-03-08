# 构建

运行 make.bat 文件

# 配置文件

## candidate_data.txt 

每一行表示参与者的名字，如果没有名字，可以用CandidateGenerator.exe自动生成数字

生成方式：
```cmd
CandidateGenerator.exe [num]   num表示参与者的数量
```

## prize_amount.txt

每一行表示奖品数量，从上到下依次对应一等奖、二等奖等。

## prize_content.txt

每一行表示奖品内容，从上到下依次对应一等奖、二等奖等。

## prize_name.txt

每一行表示奖品的名字，例如一等奖、二等奖等。从上到下依次对应一等奖、二等奖等。

# 使用方法

## 抽奖

首先按照上述内容编写好配置文件。

### 如果已经配置好candidate_data.txt

打开 Draw.exe ，然后单击抽奖，在配置文件中有多少种奖，就有几次抽奖的次数。

程序确保不会有同一个人抽到两次。

### 如果没有配置好candidate_data.txt

运行 runner.bat ，输入参与者人数，它会自动调用 CandidateGenerator.exe 来生成配置文件，然后开启 Draw.exe

## 输出

输出在 Output.txt 中，从上到下依次对应n等奖、n-1等奖等。
