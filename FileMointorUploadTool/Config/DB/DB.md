#数据库表设计
## t_files_change_log 记录所有文件改动的日志
* id 自增类型
* log_dt 日志时间
* change_type 1：add 2:update 4:delete 8:rename 16:modify
* before_value 之前名字
* after_value 之后名字

---
##t_queue
* id 自增
* status 状态 0：未处理 1，正在处理
* path 路径

---
##t_upload
* id 自增
* title 标题
* fingerprint 指纹
