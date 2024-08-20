

library(ggplot2)
library(ggpubr)
library(tidyverse)
library(broom)
library(AICcmodavg)


vis.data <- read.csv("C:/Users/ryanp/Downloads/allData.csv", header = TRUE, colClasses = c("factor", "factor", "factor","factor","numeric", "numeric"))

summary(vis.data)

one.way1 <- aov(Time ~ Vis, data = vis.data)
summary(one.way1)
one.way2 <- aov(Errors ~ Vis, data = vis.data)
summary(one.way2)


two.way1 <- aov(Time ~ Vis + Density, data = vis.data)
summary(two.way1)
two.way2 <- aov(Errors ~ Vis + Density, data = vis.data)
summary(two.way2)


interaction1 <- aov(Time ~ Density*Vis, data = vis.data)
summary(interaction1)
interaction2 <- aov(Errors ~ Density*Vis, data = vis.data)
summary(interaction2)


three.way1 <- aov(Time ~ Vis + Density + Range, data = vis.data)
summary(three.way1)
three.way2 <- aov(Errors ~ Vis + Density + Range, data = vis.data)
summary(three.way2)


interthree1 <- aov(Time ~ Density*Vis + Range, data = vis.data)
summary(interthree1)
interthree2 <- aov(Errors ~ Density*Vis + Range, data = vis.data)
summary(interthree2)




library(AICcmodavg)
model.set <- list(two.way1, three.way1, interaction1,interthree1)
model.names <- c("two.way", "three.way", "interaction","interthree")
aictab(model.set, modnames = model.names)

model.set2 <- list(two.way2, three.way2, interaction2,interthree2)
aictab(model.set2, modnames = model.names)


summary(interaction1)
summary(interaction2)






